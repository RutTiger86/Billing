using System.Runtime.CompilerServices;

namespace Billing.Api.Models.Requests
{
    public class ValidateBillTxRequest
    {
        public long BillTxId { get; set; }
        public  required string ProductId { get; set; }
        public required string PurchaseToken { get; set; }
    }
}
