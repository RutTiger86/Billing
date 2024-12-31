using Billing.Core.Enums;

namespace Billing.Core.Models
{
    public class BillDetail
    {
        public long Id { get; set; }
        public long BillTxId { get; set; }        
        public long ProductId { get; set; }
        public long AccountId { get; set; }
        public long? CharId {get; set; }
        public string? CharName { get; set; }
        public BillTxSubTypes SubType { get; set; }
        public BillTxStatus Status { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
