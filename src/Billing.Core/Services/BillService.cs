using Billing.Core.Exceptions;
using Billing.Core.Interfaces;
using Billing.Core.Models.DataBase;
using Billing.Protobuf.Core;
using Billing.Protobuf.Product;
using Billing.Protobuf.Purchase;
using Microsoft.Extensions.Logging;

namespace Billing.Core.Services
{
    public class BillService(IBillTxService billTxService, IDataService dataService, ILogger<BillService> logger,
        GoogleValidationService googlValidationServices, PointValidationService pointValidationService) : IBillService
    {
        private readonly IDataService dataService = dataService;
        private readonly ILogger<BillService> logger = logger;
        private readonly GoogleValidationService googlValidationServices = googlValidationServices;
        private readonly PointValidationService pointValidationService = pointValidationService;
        private readonly IBillTxService billTxService = billTxService;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task<(bool Result, BillingError error)> PurchaseValidation(PurchaseInfo purchaseInfo)
        {
            try
            {
                //Tx 검증
                (var txResult, var billError) = await ValidationTxProcess(purchaseInfo);
                if(!txResult)
                {
                    return (false, billError);
                }

                //거래 기록 생성 
                 var billDetailId = CreateBillDetail(purchaseInfo);
                if (billDetailId<1)
                {
                    return (false, BillingError.BillCreateDetailFailed);
                }

                // 검증 및 포인트 차감
                bool validatedResult = await ValidationProcess(billDetailId, purchaseInfo);
                if (!validatedResult)
                {
                    return (false, BillingError.PurchaseValidationFailed);
                }

                //포인트 구매시 포인트 충전 
                var product = dataService.SelectProductInfoByProductKey(purchaseInfo.ProductKey);
                var pointProduct = product.Items.Where(p => p.ProductType == ProductTypes.Point).ToList();
                if (pointProduct.Count>0)
                {
                    dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.PointChargeStart);
                    bool chargePointResult = pointValidationService.ChargePointProcess(pointProduct, purchaseInfo);

                    if(!chargePointResult)
                    {
                        return (false, BillingError.PurchaseValidationFailed);
                    }
                }

                return (true, BillingError.None);
            }
            catch (BillingException ex)
            {
                logger.LogError($"Bill Exception Error : {ex.Message}");
                return (false, BillingError.SystemError);
            }
            catch (Exception ex)
            {
                logger.LogError($"Bill Exception Error : {ex.Message}");
                return (false, BillingError.SystemError);
            }
        }
       
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task<(SubScriptionState Statue, BillingError error)> SubScriptionStateValidation(long billTxID)
        {
            try
            {
                SubScriptionState statue = SubScriptionState.Unspecified;
                var billTx = dataService.SelectBillTx(billTxID);

                if (billTx == null)
                {
                    return (statue, BillingError.TxNotFound);
                }

                var subscriptionInfo = dataService.SelectSubscriptionInfo(billTxID);

                if (subscriptionInfo == null)
                {
                    return (statue, BillingError.SubscriptionNotffound);
                }

                IValidationService validationService = GetValidationService(billTxID);

                if (validationService == null)
                {
                    return (statue, BillingError.TxUnverifiableType);
                }

                statue = await  validationService.SubscriptionsValidate(billTx.PurchaseToken);

                switch (statue)
                {
                    case SubScriptionState.Expired :
                        dataService.ExpireSubscription(subscriptionInfo.Id);
                            break;
                };

                return (statue, BillingError.None);
            }
            catch (BillingException ex)
            {
                logger.LogError($"Bill Exception Error : {ex.Message}");
                return (SubScriptionState.Unspecified, BillingError.SystemError);
            }
            catch (Exception ex)
            {
                logger.LogError($"Bill Exception Error : {ex.Message}");
                return (SubScriptionState.Unspecified, BillingError.SystemError);

            }
        }
       
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task<(bool Result, BillingError error)> CanclePurchase(long billTxId)
        {
            var billtx = dataService.SelectBillTx(billTxId);

            if(billtx == null)
            {
                return (false, BillingError.TxNotFound);
            }

            pointValidationService.PointRollBack(billTxId);

            return (true, BillingError.None);
        }

        /// <summary>
        /// 상품 트랜젝션 검증 
        /// 트랜젝션 정보에 구매 토큰 업데이트 
        /// </summary>
        private async Task<(bool IsValide, BillingError Error)> ValidationTxProcess(PurchaseInfo purchaseInfo)
        {
            var (isValid, validationError) = billTxService.ValidateBillTx(purchaseInfo.BillTxId);
            if (!isValid)
            {
                return (false, validationError);
            }

            if (!billTxService.RegistPurchaseToken(purchaseInfo.BillTxId, purchaseInfo.PurchaseToken))
            {
                return (false, BillingError.SystemError);
            }

            return (true, BillingError.None);
        }

        /// <summary>
        /// 상품 검증 프로세스 
        /// 검증 서비스 선정 및 검증진행 포인트구매시 소비처리 진행 
        /// </summary>
        private async Task<bool> ValidationProcess(long billDetailId, PurchaseInfo purchaseInfo)
        {
            IValidationService validationService = GetValidationService(purchaseInfo.BillTxId);

            if (validationService == null)
            {
                throw new BillingException(BillingError.TxUnverifiableType, "Undefined Tx Type");
            }

            return purchaseInfo.ProductType switch
            {
                BillProductType.Consumable or BillProductType.NonConsumable
                => await validationService.PurchaseProductValidate(billDetailId, purchaseInfo),

                BillProductType.SubscriptionNonAuto or BillProductType.SubscriptionAuto
                => await validationService.PruchaseSubscriptionsValidate(billDetailId, purchaseInfo),

                _ => false
            };
        }      

        /// <summary>
        /// 트랜잭션 Id로 트랜잭션 타입 선정
        /// </summary>
        private BillTxTypes GetBillTxType(long billTxId)
        {
            BillTx billTx = dataService.SelectBillTx(billTxId);
            return billTx == null ? throw new BillingException(BillingError.TxNotFound, "Transaction does not exist") : billTx.Type;
        }

        /// <summary>
        /// 트랜잭션 Id로 검증 서비스 선정
        /// </summary>
        /// <param name="billTxId"></param>
        /// <returns></returns>
        private IValidationService GetValidationService(long billTxId)
        {
            var billType = GetBillTxType(billTxId);
            IValidationService validationService = billType switch
            {
                BillTxTypes.IapGoogle => googlValidationServices,
                BillTxTypes.Point =>  pointValidationService,
                _ => null,
            };
            return validationService;
        }

        /// <summary>
        /// 거래 기록 정보 생성 
        /// </summary>
        private long CreateBillDetail(PurchaseInfo purchaseInfo)
        {
            var product =  dataService.SelectProduct(purchaseInfo.ProductKey);

            if (product == null)
            {
                return -1;
            }

            BillDetail billDetail = new()
            {
                ProductId = product.Id,
                AccountId = purchaseInfo.AccountId,
                CharId = purchaseInfo.CharId,
                CharName = purchaseInfo.CharName,
                BillTxId = purchaseInfo.BillTxId,
                SubType = purchaseInfo.ProductType,
            };

            return dataService.InsertBillDetail(billDetail);
        }
        
    }
}
