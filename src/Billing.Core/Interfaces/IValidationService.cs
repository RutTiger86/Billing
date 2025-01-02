using Billing.Core.Enums;
using Billing.Core.Models;

namespace Billing.Core.Interfaces
{
    public interface IValidationService
    {
        public Task<bool> Validate(long billDetailId, PurchaseInfo purchaseInfo);
        public Task<bool> SubscriptionsValidate(long billDetailId, PurchaseInfo purchaseInfo);
    }
}
