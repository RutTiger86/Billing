using Billing.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Models.DataBase
{
    public class PointHistory
    {
        public long Id { get; set; }
        public long BillTxId { get; set; }
        public long ProductId { get; set; }
        public PointOperationType PointOperationType { get; set; }
        public long AccountId { get; set; }
        public PointType PointType { get; set; }
        public long BeforeBalance { get; set; }
        public long Amount { get; set; }        
        public long AfterBalance { get; set; }
        public bool IsRollBack { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
