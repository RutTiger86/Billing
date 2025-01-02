using Billing.Core.Enums;
using Billing.Core.Models;

namespace Billing.Core.Interfaces
{
    public interface IBillService
    {
        Task<(bool Result, BillingError error)> PurchaseValidation(PurchaseInfo purchaseInfo);

        bool CompleteBillDetail(long billTxId);
    }
}
