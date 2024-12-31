namespace Billing.Core.Interfaces
{
    public interface IValidationService
    {
        public Task<bool> ValidatePurchaseAsync(string productId, string purchaseToken);
    }
}
