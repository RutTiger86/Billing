using Billing.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Models
{
    public class PurchaseInfo:BaseModel
    {
        public long BillTxId { get; set; }
        public BillProductType ProductType { get; set; }
        public required string ProductKey { get; set; }
        public required string PurchaseToken { get; set; }
        public long AccountId { get; set; }
        public long? CharId { get; set; }
        public string? CharName { get; set; }
        public BillDetailTypes SubType { get; set; }
        public List<PointPurchase>? PointPurchases { get; set; }
    }

}
