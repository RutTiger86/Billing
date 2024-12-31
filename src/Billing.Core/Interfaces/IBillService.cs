using Billing.Core.Enums;

namespace Billing.Core.Interfaces
{
    public interface IBillService
    {
        Task<(bool Result, BillingError error)> Validation(long billTxId, string ProductId, string PurchaseToken);
    }
}
