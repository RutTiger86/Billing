using Billing.Core.Enums;
using Billing.Core.Models;

namespace Billing.Core.Interfaces
{
    public interface IValidationService
    {
        public Task<bool> PurchaseProductValidate(long billDetailId, PurchaseInfo purchaseInfo);
        public Task<bool> PruchaseSubscriptionsValidate(long billDetailId, PurchaseInfo purchaseInfo);
        public Task<SubScriptionState> SubscriptionsValidate(string purchaseToken);
    }
}
