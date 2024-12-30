using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Interfaces
{
    public interface IBillingService
    {
        public Task<bool> ValidatePurchaseAsync(string productId, string purchaseToken);
    }
}
