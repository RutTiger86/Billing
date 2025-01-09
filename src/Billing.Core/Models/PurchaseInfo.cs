using Billing.Core.Enums;

namespace Billing.Core.Models
{
    /// <summary>
    /// 구매 요청 정보 
    /// </summary>
    public class PurchaseInfo:BaseModel
    {
        /// <summary>
        /// 트랜잭션 Id
        /// </summary>
        public long BillTxId { get; set; }
        /// <summary>
        /// 상품 타입 정보 
        /// </summary>
        public BillProductType ProductType { get; set; }
        /// <summary>
        /// 상품 Key
        /// </summary>
        public required string ProductKey { get; set; }
        /// <summary>
        /// 구매 토큰 
        /// </summary>
        public required string PurchaseToken { get; set; }
        /// <summary>
        /// 구매 대상 계정 ID
        /// </summary>
        public long AccountId { get; set; }
        /// <summary>
        /// 구매 대상 캐릭터 ID (캐릭터 대상일시) 
        /// </summary>
        public long? CharId { get; set; }
        /// <summary>
        /// 구매 대상 캐릭터 명( 캐릭터 대상일시) 
        /// </summary>
        public string? CharName { get; set; }
        /// <summary>
        /// 포인트 구매시 포인트 차감 요청 내역 
        /// </summary>
        public List<PointPurchase>? PointPurchases { get; set; }
    }

}
