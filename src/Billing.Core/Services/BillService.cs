using Billing.Core.Enums;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Billing.Core.Models.DataBase;
using Microsoft.Extensions.Logging;

namespace Billing.Core.Services
{
    public class BillService(IDataService dataService, ILogger<BillService> logger, GoogleValidationService googlValidationServices) : IBillService
    {
        private readonly IDataService dataService = dataService;
        private readonly ILogger<BillService> logger = logger;
        private readonly GoogleValidationService googlValidationServices = googlValidationServices;

        public async Task<(bool Result, BillingError error)> Validation(PurchaseInfo purchaseInfo)
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

                bool purcaseResult = await validationService.ValidatePurchase(billDetailId, purchaseInfo.ProductKey, purchaseInfo.PurchaseToken);

                if (!purcaseResult)
                {
                    return (false, BillingError.PURCHASE_VALIDATION_FAILED);
                }

                return (true, BillingError.NONE);
            }
            catch (BillingException ex)
            {
                logger.LogError($"Bill Excepion Error : {ex.Message}");
                return (false, BillingError.SYSTEM_ERROR);
            }
            catch (Exception ex)
            {
                logger.LogError($"Bill Excepion Error : {ex.Message}");
                return (false, BillingError.SYSTEM_ERROR);

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
                IsCompleted = false,
                IsDeleted = false,
            };

            return dataService.InsertBillDetail(billDetail);
        }
    }
}
