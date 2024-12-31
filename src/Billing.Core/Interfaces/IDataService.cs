using Billing.Core.Enums;
using Billing.Core.Models;

namespace Billing.Core.Interfaces
{
    public interface IDataService
    {
        public long InsertBillTx(BillTx billTx);
        public bool UpdateBillTx(long billTxId, BillTxStatus status);
        public bool UpdateBillTx(long billTxId, BillTxStatus status, bool isComplete);
        public bool UpdateBillTx(long billTxId, bool isDeleted);
        public bool UpdateBillTx(long billTxId, string purchaseToken);
        public bool InsertBillTxDetails(BillDetail billDetail);
        public bool UpdateBillTxDetailsDetail(long billDetailId);
        public BillTx GetBillTx(long billTxId, bool isDeleted = false);
        public BillDetail GetBillDetails(long billTxId);
        public BillDetail GetBillDetail(long billDetailId);

    }
}
