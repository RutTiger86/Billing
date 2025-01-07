namespace Billing.Core.Models.DataBase
{
    public class Product
    {
        public long Id { get; set; }
        public required string ProductKey { get; set; }
        public required string ProductName { get; set; }
        public required int Price { get; set; }
        public bool IsUse { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
