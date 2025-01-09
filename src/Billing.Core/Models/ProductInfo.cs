namespace Billing.Core.Models
{
    /// <summary>
    /// 상품 정보 
    /// </summary>
    public class ProductInfo
    {        
        /// <summary>
        /// 상품 Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 상품 Key
        /// </summary>
        public required string ProductKey { get; set; }
        /// <summary>
        /// 상품명
        /// </summary>
        public required string ProductName { get; set; }
        /// <summary>
        /// 사용여부 
        /// </summary>
        public bool IsUse { get; set; }
        /// <summary>
        /// 상품 별 아이템 정보 
        /// </summary>
        public required List<ProductItemInfo> Items { get; set; }
    }
}
