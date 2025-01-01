using Billing.Core.Enums;
using Billing.Core.Models.DataBase;

namespace Billing.Core.Interfaces
{
    public interface IDataService
    {
        public long InsertBillTx(BillTx billTx);
        public bool UpdateBillTx(long billTxId, bool isComplete);
        public bool DeleteBillTx(long billTxId, bool isDeleted);
        public bool UpdateBillTx(long billTxId, string purchaseToken);
        public long InsertBillDetail(BillDetail billDetail);
        public bool UpdateBillDetail(long billDetailId, BillTxStatus Status);
        public BillTx GetBillTx(long billTxId, bool isDeleted = false);
        public List<BillDetail> GetBillDetails(long billTxId);
        public BillDetail GetBillDetail(long billDetailId);
        public Product GetProduct(string productKey, bool isUse = true);
    }
}
