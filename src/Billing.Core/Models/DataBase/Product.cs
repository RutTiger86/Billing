namespace Billing.Core.Models.DataBase
{
    public class Product
    {
        /// <summary>
        /// 상품Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 상품 키
        /// </summary>
        public required string ProductKey { get; set; }
        /// <summary>
        /// 상품 명
        /// </summary>
        public required string ProductName { get; set; }
        /// <summary>
        /// 상품 가격 (Point)
        /// </summary>
        public required int Price { get; set; }
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
