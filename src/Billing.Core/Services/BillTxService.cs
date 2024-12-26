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

        public bool UpdateBillTxState(long billTxId, BillTxStatus status)
        {
            return dataService.UpdateBillTxState(billTxId, status);
        }

        public (bool, ValidateError) ValidateBillTx(long billTxId)
        {
            BillTx billTx =  dataService.GetBillTx(billTxId);

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

        public bool CancleBillTx(long billTxId)
        {
            return dataService.UpdateBillTxState(billTxId, true);
        }

        public bool EndBillTx(long billTxId)
        {
            return dataService.UpdateBillTxState(billTxId, BillTxStatus.COMPLETED, true);
        }
    }
}
