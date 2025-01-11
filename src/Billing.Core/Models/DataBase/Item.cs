using Billing.Protobuf.Point;

namespace Billing.Core.Models.DataBase
{
    public class Item
    {
        /// <summary>
        /// 상품 Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 상품명
        /// </summary>
        public required string ItemName { get; set; }
        /// <summary>
        /// 포인트 상품 타입 (Point상품일경우) 
        /// <see cref="PointType"/>
        /// </summary>
        public PointType PointType { get; set; }
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
