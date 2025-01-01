namespace Billing.Core.Interfaces
{
    public interface IValidationService
    {
        public Task<bool> ValidatePurchase(long billDetailId, string productId, string purchaseToken);
    }
}
