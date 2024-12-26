using Billing.Core.Enums;

namespace Billing.Core.Models
{
    public class BillTx : BaseModel
    {
        public long Id { get; set; }
        public BillTxTypes Type { get; set; }
        public BillTxStatus Status { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
