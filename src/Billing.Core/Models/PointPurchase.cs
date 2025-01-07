using Billing.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Models
{
    public class PointPurchase : BaseModel
    {
        public PointType PointType { get; set; }
        public long Amount { get; set; }
    }
}
