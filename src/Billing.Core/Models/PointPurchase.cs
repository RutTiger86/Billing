using Billing.Core.Enums;

namespace Billing.Core.Models
{
    /// <summary>
    /// 포인트 처리 요청 정보
    /// </summary>
    public class PointPurchase : BaseModel
    {
        /// <summary>
        /// 포인트 타입 
        /// </summary>
        public PointType PointType { get; set; }
        /// <summary>
        /// 변경액
        /// </summary>
        public long Amount { get; set; }
    }
}
