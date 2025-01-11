using Billing.Protobuf.Point;

namespace Billing.Core.Models.DataBase
{
    public class PointHistory
    {
        /// <summary>
        /// 포인트 기록 Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 트랜젝션 Id
        /// </summary>
        public long BillTxId { get; set; }
        /// <summary>
        /// 상품 Id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 포인트 변경 타입 
        /// <see cref="PointOperationType"/>
        /// </summary>
        public PointOperationType PointOperationType { get; set; }
        /// <summary>
        /// 계정 Id
        /// </summary>
        public long AccountId { get; set; }
        /// <summary>
        /// 포인트 타입
        /// <see cref="PointType"/>
        /// </summary>
        public PointType PointType { get; set; }
        /// <summary>
        /// 거래 이전 잔액 
        /// </summary>
        public long BeforeBalance { get; set; }
        /// <summary>
        /// 변동액 
        /// </summary>
        public long Amount { get; set; }        
        /// <summary>
        /// 거래 이후 잔액 
        /// </summary>
        public long AfterBalance { get; set; }
        /// <summary>
        /// 롤백 여부 
        /// </summary>
        public bool IsRollBack { get; set; }
        /// <summary>
        /// 변경일
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 등록일 
        /// </summary>
        public DateTime CreateDate { get; set; }

    }
}
