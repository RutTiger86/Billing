using Billing.Core.Enums;

namespace Billing.Core.Models
{
    public class Product
    {
        public long Id { get; set; }
        public required string ProductName { get; set; }
        public ProductTypes Types { get; set; }
        public bool IsUse {  get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
