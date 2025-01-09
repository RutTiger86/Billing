using Billing.Core.Enums;
using Billing.Core.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Models
{
    /// <summary>
    /// 상품별 아이템 정보 
    /// <see cref="ProductInfo"/>
    /// </summary>
    public class ProductItemInfo
    {
        /// <summary>
        /// 상품별 아이템 Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 상품 Id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 상품 설정 타입 
        /// </summary>
        public ProductTypes Types { get; set; }
        /// <summary>
        /// 아이템 ID
        /// </summary>
        public long ItemId { get; set; }
        /// <summary>
        /// 아이템 수량 
        /// </summary>
        public int ItemVolume { get; set; }
        /// <summary>
        /// 아이템 명
        /// </summary>
        public required string ItemName { get; set; }
        /// <summary>
        /// 포인트 상품시 포인트 타입 
        /// </summary>
        public PointType PointType { get; set; }
        /// <summary>
        /// 사용여부 
        /// </summary>
        public bool IsUse { get; set; }
    }
}
