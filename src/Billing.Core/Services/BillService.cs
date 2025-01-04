using Billing.Core.Enums;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Billing.Core.Models.DataBase;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Billing.Core.Services
{
    public class BillService(IDataService dataService, ILogger<BillService> logger, GoogleValidationService googlValidationServices) : IBillService
    {
        private readonly IDataService dataService = dataService;
        private readonly ILogger<BillService> logger = logger;
        private readonly GoogleValidationService googlValidationServices = googlValidationServices;

        public async Task<(bool Result, BillingError error)> PurchaseValidation(PurchaseInfo purchaseInfo)
        {
            try
            {
                //Bill Data Process
                var billDetailId = CreateBillDetail(purchaseInfo);

                if (billDetailId<1)
                {
                    return (false, BillingError.BILL_CREATE_DETAIL_FAILED);
                }

                IValidationService validationService = GetValidationService(purchaseInfo.BillTxId);

                if (validationService == null)
                {
                    return (false, BillingError.TX_UNVERIFIABLE_TYPE);                    
                }

                bool purcaseResult = purchaseInfo.ProductType switch
                { 
                    BillProductType.CONSUMABLE or BillProductType.NON_CONSUMABLE => await validationService.PurchaseProductValidate(billDetailId, purchaseInfo),                
                    BillProductType.SUBSCRIPTION => await validationService.PruchaseSubscriptionsValidate(billDetailId, purchaseInfo),
                    _ => false
                };

                if (!purcaseResult)
                {
                    return (false, BillingError.PURCHASE_VALIDATION_FAILED);
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
                var billTx = dataService.GetBillTx(billTxID);

                if (billTx == null)
                {
                    return (statue, BillingError.TX_NOTFFOUND);
                }

                var subscriptionInfo = dataService.GetSubscriptionInfo(billTxID);

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

        private BillTxTypes GetBillTxType(long billTxId)
        {
            BillTx billTx = dataService.GetBillTx(billTxId);
            return billTx == null ? throw new BillingException(BillingError.TX_NOTFFOUND, "Transaction does not exist") : billTx.Type;
        }

        private IValidationService GetValidationService(long billTxId)
        {
            var billType = GetBillTxType(billTxId);
            IValidationService validationService = billType switch
            {
                BillTxTypes.IAP_Google => googlValidationServices,
                _ => null,
            };
            return validationService;
        }

        private long CreateBillDetail(PurchaseInfo purchaseInfo)
        {
            var product =  dataService.GetProduct(purchaseInfo.ProductKey);

            if (product == null)
            {
                return -1;
            }

            BillDetail billDetail = new()
            {
                ProductId = product.Id,
                Status = BillTxStatus.INITIATED,
                AccountId = purchaseInfo.AccountId,
                CharId = purchaseInfo.CharId,
                CharName = purchaseInfo.CharName,
                SubType = purchaseInfo.SubType,          
                BillTxId = purchaseInfo.BillTxId,
                IsDeleted = false,
            };

            return dataService.InsertBillDetail(billDetail);
        }

        public bool CompleteBillDetail(long billTxId)
        {
            return dataService.CompleteBillDetail(billTxId);
        }
                


    }
}
