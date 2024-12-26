using Billing.Core.Enums;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Microsoft.Extensions.Logging;

namespace Billing.Core.Services
{
    public class BillTxService : IBillTxService
    {
        private IDataService dataService;
        private ILogger<BillTxService> logger;

        public BillTxService(IDataService dataService, ILogger<BillTxService> logger) 
        {
            this.dataService = dataService;
            this.logger = logger;
        }

        public long IssueBillTx(BillTxTypes transactionType)
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

        public bool UpdateBillTxState(long BillTxId, BillTxStatus status)
        {
            return dataService.UpdateBillTxState(BillTxId, status);
        }

        public (bool, ValidateError) ValidateBillTx(long BillTxId)
        {
            BillTx billTx =  dataService.GetBillTx(BillTxId);

            if (billTx == null)
            {
                return (false, ValidateError.NotFound);
            }

            if(billTx.IsDeleted)
            {
                return (false, ValidateError.NotFound);
            }

            if (billTx.IsCompleted)
            {
                return (false, ValidateError.NotFound);

            }

            return (true, ValidateError.None);
        }

        public bool CancleBillTx(long BillTxId)
        {
            return dataService.UpdateBillTxState(BillTxId, true);
        }

        public bool EndBillTx(long BillTxId)
        {
            return dataService.UpdateBillTxState(BillTxId, BillTxStatus.COMPLETED, true);
        }
    }
}
