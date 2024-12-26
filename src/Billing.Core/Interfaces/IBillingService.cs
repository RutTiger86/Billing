using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Interfaces
{
    public interface IBillingService
    {
        public bool ValidateReceipt();
    }
}
