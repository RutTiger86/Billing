using Billing.Core.Enums;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Billing.Core.Models.DataBase;
using Microsoft.Extensions.Logging;
using static Google.Apis.AndroidPublisher.v3.EditsResource;
using static Google.Apis.Requests.BatchRequest;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    ChargePointProcess(pointProduct,purchaseInfo);
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
                    return (statue, BillingError.TX_NOTFFOUND);
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

        private async Task<bool> ValidationProcess(long billDetailId, PurchaseInfo purchaseInfo)
        {
            IValidationService validationService = GetValidationService(purchaseInfo.BillTxId);

            if (validationService == null)
            {
                throw new BillingException(BillingError.TX_UNVERIFIABLE_TYPE, "Undefined Tx Type");
            }

            return purchaseInfo.ProductType switch
            {
                BillProductType.CONSUMABLE or BillProductType.NON_CONSUMABLE => await validationService.PurchaseProductValidate(billDetailId, purchaseInfo),
                BillProductType.SUBSCRIPTION => await validationService.PruchaseSubscriptionsValidate(billDetailId, purchaseInfo),
                _ => false
            };
        }
       
        private void ChargePointProcess(List<ProductItemInfo> products, PurchaseInfo purchaseInfo)
        {
            foreach (ProductItemInfo productItem in products)
            {
                var pointLedgers = dataService.SelectLedgerByPointType(purchaseInfo.AccountId, productItem.PointType);
                if(pointLedgers ==  null)
                {
                    CreatePointLedger(purchaseInfo,productItem);
                    pointLedgers = dataService.SelectLedgerByPointType(purchaseInfo.AccountId, productItem.PointType);
                }

                PointHistory pointHistory = new()
                {
                    AccountId = purchaseInfo.AccountId,
                    BillTxId = purchaseInfo.BillTxId,
                    PointOperationType = PointOperationType.CHARGE,
                    ProductId = productItem.ProductId,
                    PointType = productItem.PointType,
                    BeforeBalance = pointLedgers.Balance,
                    Amount = productItem.ItemVolume,                   
                };

                dataService.ChargeLedger(purchaseInfo.AccountId, productItem.PointType, productItem.ItemVolume);

                pointLedgers = dataService.SelectLedgerByPointType(purchaseInfo.AccountId, productItem.PointType);
                pointHistory.AfterBalance = pointLedgers.Balance;
                dataService.InsertPointHistory(pointHistory);
            }
        }

        private bool CreatePointLedger(PurchaseInfo purchaseInfo, ProductItemInfo productItem)
        {
            Ledger ledger = new()
            {
                AccountId = purchaseInfo.AccountId,
                Balance = 0,
                Type = productItem.PointType,
            };
            dataService.InsertLedger(ledger);

            PointHistory pointHistory = new()
            {
                AccountId = purchaseInfo.AccountId,
                BillTxId = purchaseInfo.BillTxId,
                PointOperationType = PointOperationType.CREATE,
                ProductId = productItem.ProductId,
                PointType = productItem.PointType,
                AfterBalance = 0,
                BeforeBalance = 0,
                Amount = 0,
            };

            dataService.InsertPointHistory(pointHistory);

            return true;
        }

        private BillTxTypes GetBillTxType(long billTxId)
        {
            BillTx billTx = dataService.SelectBillTx(billTxId);
            return billTx == null ? throw new BillingException(BillingError.TX_NOTFFOUND, "Transaction does not exist") : billTx.Type;
        }

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
                SubType = purchaseInfo.SubType,          
                BillTxId = purchaseInfo.BillTxId,
                IsDeleted = false,
            };

            return dataService.InsertBillDetail(billDetail);
        }
        
    }
}
