using Billing.Core.Enums;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Billing.Core.Models.DataBase;
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
                    return (false, BillingError.BILL_CREATE_DETAIL_FAILED);
                }

                // 검증 및 포인트 차감
                bool validatedResult = await ValidationProcess(billDetailId, purchaseInfo);
                if (!validatedResult)
                {
                    return (false, BillingError.PURCHASE_VALIDATION_FAILED);
                }

                //포인트 구매시 포인트 충전 
                var product = dataService.SelectProductInfoByProductKey(purchaseInfo.ProductKey);
                var pointProduct = product.Items.Where(p => p.Types == ProductTypes.POINT).ToList();
                if (pointProduct.Count>0)
                {
                    dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.POINT_CHARGE_START);
                    bool chargePointResult = pointValidationService.ChargePointProcess(pointProduct, purchaseInfo);

                    if(!chargePointResult)
                    {
                        return (false, BillingError.PURCHASE_VALIDATION_FAILED);
                    }
                }

                return (true, BillingError.NONE);
            }
            catch (BillingException ex)
            {
                logger.LogError($"Bill Exception Error : {ex.Message}");
                return (false, BillingError.SYSTEM_ERROR);
            }
            catch (Exception ex)
            {
                logger.LogError($"Bill Exception Error : {ex.Message}");
                return (false, BillingError.SYSTEM_ERROR);
            }
        }

        public async Task<(SubScriptionState Statue, BillingError error)> SubScriptionStateValidation(long billTxID)
        {
            try
            {
                SubScriptionState statue = SubScriptionState.SUBSCRIPTION_STATE_UNSPECIFIED;
                var billTx = dataService.SelectBillTx(billTxID);

                if (billTx == null)
                {
                    return (statue, BillingError.TX_NOT_FOUND);
                }

                var subscriptionInfo = dataService.SelectSubscriptionInfo(billTxID);

                if (subscriptionInfo == null)
                {
                    return (statue, BillingError.SUBSCRIPTION_NOTFFOUND);
                }

                IValidationService validationService = GetValidationService(billTxID);

                if (validationService == null)
                {
                    return (statue, BillingError.TX_UNVERIFIABLE_TYPE);
                }

                statue = await  validationService.SubscriptionsValidate(billTx.PurchaseToken);

                switch (statue)
                {
                    case SubScriptionState.SUBSCRIPTION_STATE_EXPIRED :
                        dataService.ExpireSubscription(subscriptionInfo.Id);
                            break;
                };

                return (statue, BillingError.NONE);
            }
            catch (BillingException ex)
            {
                logger.LogError($"Bill Exception Error : {ex.Message}");
                return (SubScriptionState.SUBSCRIPTION_STATE_UNSPECIFIED, BillingError.SYSTEM_ERROR);
            }
            catch (Exception ex)
            {
                logger.LogError($"Bill Exception Error : {ex.Message}");
                return (SubScriptionState.SUBSCRIPTION_STATE_UNSPECIFIED, BillingError.SYSTEM_ERROR);

            }
        }
       
        public async Task<(bool Result, BillingError error)> CanclePurchase(long billTxId)
        {
            var billtx = dataService.SelectBillTx(billTxId);

            if(billtx == null)
            {
                return (false, BillingError.TX_NOT_FOUND);
            }

            pointValidationService.PointRollBack(billTxId);

            return (true, BillingError.NONE);
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
                return (false, BillingError.SYSTEM_ERROR);
            }

            return (true, BillingError.NONE);
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
                throw new BillingException(BillingError.TX_UNVERIFIABLE_TYPE, "Undefined Tx Type");
            }

            return purchaseInfo.ProductType switch
            {
                BillProductType.CONSUMABLE or BillProductType.NON_CONSUMABLE 
                => await validationService.PurchaseProductValidate(billDetailId, purchaseInfo),

                BillProductType.SUBSCRIPTION_NON_AUTO or BillProductType.SUBSCRIPTION_AUTO 
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
            return billTx == null ? throw new BillingException(BillingError.TX_NOT_FOUND, "Transaction does not exist") : billTx.Type;
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
                BillTxTypes.IAP_Google => googlValidationServices,
                BillTxTypes.POINT =>  pointValidationService,
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
