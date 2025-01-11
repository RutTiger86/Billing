using Billing.Core.Interfaces;
using Billing.Core.Models.DataBase;
using Billing.Protobuf.Core;
using Microsoft.Extensions.Logging;

namespace Billing.Core.Services
{
    public class BillTxService(IDataService dataService,IBillService billService, ILogger<BillTxService> logger) : IBillTxService
    {
        private readonly IDataService dataService = dataService;
        private readonly ILogger<BillTxService> logger = logger;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public long IssueBillTx(BillTxTypes transactionType)
        {
            try
            {
                BillTx transaction = new ()
                {
                    Type = transactionType,
                    Status = BillTxStatus.Initiated,
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public (bool IsValide, BillingError Error) ValidateBillTx(long billTxId)
        {
            try
            {
                BillTx billTx = dataService.SelectBillTx(billTxId);

                if (billTx == null)
                {
                    return (false, BillingError.TxNotFound);
                }

                if (billTx.IsDone)
                {
                    return (false, BillingError.TxAlreadyIsDone);
                }

                return (true, BillingError.None);
            }
            catch (Exception ex)
            {
                logger.LogError($"ValidateBillTx Error: {ex.ToString()}");
                return (false, BillingError.SystemError);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool CancleBillTx(long billTxId)
        {
            try
            {
                return dataService.UpdateBillTxStatus(billTxId, BillTxStatus.Cancled, true);
            }
            catch (Exception ex)
            {
                logger.LogError($"CancleBillTx Error: {ex.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool EndBillTx(long billTxId)
        {
            try
            {
                return dataService.UpdateBillTxStatus(billTxId, BillTxStatus.Completed, true);
            }
            catch (Exception ex)
            {
                logger.LogError($"CancleBillTx Error: {ex.ToString()}");
                return false;
            }
        }
    }
}
