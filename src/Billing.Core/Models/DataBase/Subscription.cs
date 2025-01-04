using Billing.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Models.DataBase
{
    public class SubscriptionInfo
    {        
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long BillTxId { get; set; }
        public long BillDetailId { get; set; }
        public long? ExpiryTimeMillis { get; set; }
        public SubScriptionState State { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
