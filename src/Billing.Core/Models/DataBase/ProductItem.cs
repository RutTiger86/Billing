using Billing.Core.Enums;

namespace Billing.Core.Models.DataBase
{
    /// <summary>
    ///  상품과 아이템 연결 테이블 
    ///  <see cref="Product"/>
    ///  <see cref="Item"/>
    /// </summary>
    public class ProductItem
    {
        /// <summary>
        /// 상품별 아이템 ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 상품 Id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 아이템 구매 타입 
        /// <see cref="ProductTypes"/>
        /// </summary>
        public ProductTypes Types { get; set; }
        /// <summary>
        /// 아이템 Id
        /// </summary>
        public long ItemId { get; set; }
        /// <summary>
        /// 지급 Item 수량 
        /// </summary>
        public int ItemVolume { get; set; }
        /// <summary>
        /// 사용여부 
        /// </summary>
        public bool IsUse { get; set; }
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
