using Billing.Core.Enums;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Microsoft.Extensions.Logging;

namespace Billing.Core.Services
{
    public class BillService(IDataService dataService, ILogger<BillService> logger, GoogleValidationService googlValidationServices) : IBillService
    {
        private IDataService dataService = dataService;
        private ILogger<BillService> logger = logger;
        private GoogleValidationService googlValidationServices = googlValidationServices;

        public async Task<(bool Result, BillingError error)> Validation(long billTxId, string ProductId, string PurchaseToken)
        {
            try
            {
                IValidationService validationService = GetValidationService(billTxId);

                if (validationService != null)
                {
                    bool purcaseResult = await validationService.ValidatePurchaseAsync(ProductId, PurchaseToken);

                    if (!purcaseResult)
                    {
                        return (false, BillingError.PURCHASE_VALIDATION_FAILED);
                    }

                    //Bill Data Process

                    return (true, BillingError.NONE);
                }
                else
                {
                    return (false, BillingError.TX_UNVERIFIABLE_TYPE);
                }
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
    }
}
