using Billing.Core.Enums;
using Billing.Core.Models;

namespace Billing.Core.Interfaces
{
    public interface IBillService
    {
        Task<(bool Result, BillingError error)> PurchaseValidation(PurchaseInfo purchaseInfo);
        Task<(SubScriptionState Statue, BillingError error)> SubScriptionStateValidation(long billTxID);
        bool CompleteBillDetail(long billTxId);


    }
}
