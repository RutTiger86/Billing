using Billing.Core.Enums;

namespace Billing.Core.Models.DataBase
{
    public class ProductItem
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public ProductTypes Types { get; set; }
        public long ItemId { get; set; }
        public int ItemVolume { get; set; }
        public bool IsUse { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
