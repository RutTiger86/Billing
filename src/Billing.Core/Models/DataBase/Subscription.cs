using Billing.Core.Enums;

namespace Billing.Core.Models.DataBase
{
    public class SubscriptionInfo
    {        
        /// <summary>
        /// 구독 정보 ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 구독 대상 계정 Id
        /// </summary>
        public long AccountId { get; set; }
        /// <summary>
        /// 트랜젝션 ID
        /// </summary>
        public long BillTxId { get; set; }
        /// <summary>
        /// 거래 기록 Id
        /// </summary>
        public long BillDetailId { get; set; }
        /// <summary>
        /// 만료일 UnixTime
        /// </summary>
        public long? ExpiryTimeMillis { get; set; }
        /// <summary>
        /// 구독 상태 
        /// </summary>
        public SubScriptionState State { get; set; }
        /// <summary>
        /// 만료여부
        /// </summary>
        public bool IsExpired { get; set; }
        /// <summary>
        /// 등록일
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 변경일
        /// </summary>
        public DateTime UpdateDate { get; set; }

    }
}
