using Billing.Core.Enums;
using Billing.Core.Models.DataBase;

namespace Billing.Core.Interfaces
{
    public interface IDataService
    {
        public long InsertBillTx(BillTx billTx);
        public long InsertBillDetail(BillDetail billDetail);
        public long InsertSubscriptionInfo(SubscriptionInfo subscriptionInfo);

        public bool UpdateBillTx(long billTxId, bool isComplete);
        public bool DeleteBillTx(long billTxId, bool isDeleted);
        public bool UpdateBillTx(long billTxId, string purchaseToken);
        public bool UpdateBillDetail(long billDetailId, BillTxStatus status);
        public bool CompleteBillDetail(long billlTxId);
        public bool ExpireSubscription(long subscriptionId);

        public BillTx GetBillTx(long billTxId, bool isDeleted = false);

        public List<BillDetail> GetBillDetails(long billTxId);
        public BillDetail GetBillDetail(long billDetailId);
        public Product GetProduct(string productKey, bool isUse = true);
        public SubscriptionInfo GetSubscriptionInfo(long billTxId);
    }
}
