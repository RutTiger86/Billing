using Billing.Core.Enums;

namespace Billing.Core.Interfaces
{
    public interface IBillTxService
    {
        public long IssueBillTx(BillTxTypes transactionType);

        public (bool IsValide, BillingError Error) ValidateBillTx(long billTxId);

        public bool UpdateBillTxState(long billTxId, BillTxStatus status);

        public bool RegistPurchaseToken(long billTxId, string purchaseToken);

        public bool EndBillTx(long billTxId);

        public bool CancleBillTx(long billTxId);

    }
}
