using Billing.Protobuf.Core;

namespace Billing.Core.Interfaces
{
    public interface IBillTxService
    {

        /// <summary>
        /// 거래 트랜잭션 발급 
        /// </summary>
        public long IssueBillTx(BillTxTypes transactionType);

        /// <summary>
        /// 거래 트랜잭션 상태 검증
        /// </summary>
        public (bool IsValide, BillingError Error) ValidateBillTx(long billTxId);

        /// <summary>
        /// 거래 토큰 기록 
        /// </summary>
        public int RegistPurchaseToken(long billTxId, string purchaseToken);

        /// <summary>
        /// 거래 트랜잭션 종료 
        /// </summary>
        public int EndBillTx(long billTxId);

        /// <summary>
        /// 거래 트랜잭션 취소 
        /// </summary>
        public int CancelBillTx(long billTxId);

    }
}
