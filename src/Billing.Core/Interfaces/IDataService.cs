using Billing.Core.Enums;
using Billing.Core.Models;
using Billing.Core.Models.DataBase;

namespace Billing.Core.Interfaces
{
    public interface IDataService
    {
        public long InsertBillTx(BillTx billTx);
        public long InsertBillDetail(BillDetail billDetail);
        public long InsertSubscriptionInfo(SubscriptionInfo subscriptionInfo);
        public long InsertLedger(Ledger ledger);

        public bool CompleteBillTx(long billTxId);
        public bool DeleteBillTx(long billTxId, bool isDeleted);
        public bool UpdateBillTxToken(long billTxId, string purchaseToken);
        public bool UpdateBillTxStatus(long billTxId, BillTxStatus status);
        public bool ExpireSubscription(long subscriptionId);

        public bool ChargeLedger(long accountId, PointType pointType, long amount);

        public BillTx SelectBillTx(long billTxId, bool isDeleted = false);
        public List<Ledger> SelectLedger(long accountId);
        public List<BillDetail> SelectBillDetails(long billTxId);
        public BillDetail SelectBillDetail(long billDetailId);
        public Product SelectProduct(string productKey, bool isUse = true);
        public SubscriptionInfo SelectSubscriptionInfo(long billTxId);
        public ProductInfo SelectProductInfoByProductKey(string productKey);
    }
}
