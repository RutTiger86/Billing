using Billing.Protobuf.Point;

namespace Billing.Core.Models.DataBase
{
    public class Ledger
    {
        /// <summary>
        /// 지갑 Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 지갑 소유자 계정 Id
        /// </summary>
        public long AccountId {get; set; }
        /// <summary>
        /// 포인트 타입 
        /// <see cref="PointType"/>
        /// </summary>
        public PointType Type {get; set; }
        /// <summary>
        /// 보유 잔액 
        /// </summary>
        public long Balance {get; set; }
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
