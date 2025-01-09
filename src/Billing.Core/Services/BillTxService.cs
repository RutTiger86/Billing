using Billing.Core.Enums;
using Billing.Core.Interfaces;
using Billing.Core.Models.DataBase;
using Microsoft.Extensions.Logging;

namespace Billing.Core.Services
{
    public class BillTxService(IDataService dataService,IBillService billService, ILogger<BillTxService> logger) : IBillTxService
    {
        private IDataService dataService = dataService;
        private ILogger<BillTxService> logger = logger;

        public long IssueBillTx(BillTxTypes transactionType)
        {
            try
            {
                BillTx transaction = new ()
                {
                    Type = transactionType,
                    Status = BillTxStatus.INITIATED,
                    IsDone = false,
                };

                long transactionID = dataService.InsertBillTx(transaction);
                return transactionID;
            }
            catch (Exception ex)
            {
                logger.LogError($"IssueBill Error: {ex.ToString()}");
                return -1;
            }
        }

        public (bool IsValide, BillingError Error) ValidateBillTx(long billTxId)
        {
            try
            {
                BillTx billTx = dataService.SelectBillTx(billTxId);

                if (billTx == null)
                {
                    return (false, BillingError.TX_NOT_FOUND);
                }

                if (billTx.IsDone)
                {
                    return (false, BillingError.TX_ALREADY_IS_DONE);
                }

                return (true, BillingError.NONE);
            }
            catch (Exception ex)
            {
                logger.LogError($"ValidateBillTx Error: {ex.ToString()}");
                return (false, BillingError.SYSTEM_ERROR);
            }
        }

        public bool RegistPurchaseToken(long billTxId, string purchaseToken)
        {
            try
            {
                return dataService.UpdateBillTxToken(billTxId, purchaseToken);
            }
            catch (Exception ex)
            {
                logger.LogError($"CancleBillTx Error: {ex.ToString()}");
                return false;
            }
        }

        public bool CancleBillTx(long billTxId)
        {
            try
            {
                return dataService.UpdateBillTxStatus(billTxId, BillTxStatus.CANCLED, true);
            }
            catch (Exception ex)
            {
                logger.LogError($"CancleBillTx Error: {ex.ToString()}");
                return false;
            }
        }

        public bool EndBillTx(long billTxId)
        {
            try
            {
                return dataService.UpdateBillTxStatus(billTxId, BillTxStatus.COMPLETED, true);
            }
            catch (Exception ex)
            {
                logger.LogError($"CancleBillTx Error: {ex.ToString()}");
                return false;
            }
        }
    }
}
