using Billing.Protobuf.Core;

namespace Billing.Api.Models.Requests
{
    public class ValidateBillTxRequest
    {
        public long BillTxId { get; set; }
        public  required string ProductId { get; set; }
        public required string PurchaseToken { get; set; }
        public long AccountId { get; set; }
        public long? CharId { get; set; }
        public string? CharName { get; set; }
        public BillProductType ProductType { get; set; }
    }
}
