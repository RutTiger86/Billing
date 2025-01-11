using Billing.Core.Models.DataBase;
using Billing.Protobuf.Core;
using Billing.Protobuf.Point;
using Billing.Protobuf.Product;

namespace Billing.Core.Interfaces
{
    public interface IDataService
    {
        public long InsertBillTx(BillTx billTx);
        public long InsertBillDetail(BillDetail billDetail);
        public long InsertSubscriptionInfo(SubscriptionInfo subscriptionInfo);
        public long InsertLedger(Ledger ledger);
        public long InsertPointHistory(PointHistory pointHistory);

        public bool UpdateBillTxToken(long billTxId, string purchaseToken);
        public bool UpdateBillTxStatus(long billTxId, BillTxStatus status, bool IsDone = false);
        public bool ExpireSubscription(long subscriptionId);
        public bool UpdatePointHistoryIsRollBack(long pointHistoryId);

        public bool ChargeLedger(long accountId, PointType pointType, long amount);
        public bool Withdrawledger(long accountId, PointType pointType, long amount);


        public BillTx SelectBillTx(long billTxId);
        public BillDetail SelectBillDetail(long billDetailId);
        public List<BillDetail> SelectBillDetails(long billTxId);
        public List<Ledger> SelectLedger(long accountId);
        public Ledger SelectLedgerByPointType(long accountId, PointType pointType);
        public Product SelectProduct(string productKey, bool isUse = true);
        public SubscriptionInfo SelectSubscriptionInfo(long billTxId);
        public ProductInfo SelectProductInfoByProductKey(string productKey);
        public List<PointHistory> SelectPointHistories(long billTxId);
    }
}
