using Billing.Core.Enums;

namespace Billing.Core.Models.DataBase
{
    public class BillDetail
    {
        /// <summary>
        /// 거래 기록 ID 
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 트랜잭션 ID
        /// </summary>
        public long BillTxId { get; set; }
        /// <summary>
        /// 상품 ID
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 계정 ID
        /// </summary>
        public long AccountId { get; set; }
        /// <summary>
        /// 캐릭터 ID
        /// </summary>
        public long? CharId { get; set; }
        /// <summary>
        /// 캐릭명 
        /// </summary>
        public string? CharName { get; set; }
        /// <summary>
        /// 상품 타입
        /// <see cref="BillProductType"/>
        /// </summary>
        public BillProductType SubType { get; set; }
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
