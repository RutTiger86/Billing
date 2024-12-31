using Billing.Core.Enums;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Microsoft.Extensions.Logging;

namespace Billing.Core.Services
{
    public class BillTxService(IDataService dataService, ILogger<BillTxService> logger) : IBillTxService
    {
        private IDataService dataService = dataService;
        private ILogger<BillTxService> logger = logger;

        public long IssueBillTx(BillTxTypes transactionType)
        {
            try
            {
                BillTx transaction = new BillTx()
                {
                    Status = BillTxStatus.INITIATED,
                    Type = transactionType,
                    IsDeleted = false,
                    IsCompleted = false,
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

        public bool UpdateBillTxState(long billTxId, BillTxStatus status)
        {
            try
            {
                return dataService.UpdateBillTx(billTxId, status);
            }
            catch (Exception ex)
            {
                logger.LogError($"UpdateBillTxState Error: {ex.ToString()}");
                return false;
            }
        }

        public (bool IsValide, BillingError Error) ValidateBillTx(long billTxId)
        {
            try
            {
                BillTx billTx = dataService.GetBillTx(billTxId);

                if (billTx == null)
                {
                    return (false, BillingError.TX_NOTFFOUND);
                }

                if (billTx.IsCompleted)
                {
                    return (false, BillingError.TX_ALREADY_COMPLETED);
                }

                if (billTx.Status != BillTxStatus.INITIATED)
                {
                    return (false, BillingError.TX_ALREADY_INPROGRESS);
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
                return dataService.UpdateBillTx(billTxId, purchaseToken);
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
                return dataService.UpdateBillTx(billTxId, true);
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
                return dataService.UpdateBillTx(billTxId, BillTxStatus.COMPLETED, true);
            }
            catch (Exception ex)
            {
                logger.LogError($"CancleBillTx Error: {ex.ToString()}");
                return false;
            }
        }
    }
}
