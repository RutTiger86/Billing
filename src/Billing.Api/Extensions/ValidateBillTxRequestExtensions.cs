using Billing.Api.Middlewares;
using Billing.Api.Models.Requests;
using Billing.Core.Models;

namespace Billing.Api.Extensions
{
    public static class ValidateBillTxRequestExtensions
    {
        public static PurchaseInfo toPurchaseInfo(this ValidateBillTxRequest request)
        {
            return new PurchaseInfo()
            {
                ProductKey = request.ProductId,
                PurchaseToken = request.PurchaseToken,
                AccountId = request.AccountId,
                BillTxId = request.BillTxId,
                CharId = request.CharId,
                CharName = request.CharName,
                ProductType = request.ProductType,
            };
        }
    }
}
