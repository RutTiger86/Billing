using Billing.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Models
{
    public class ProductDetail
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public ProductTypes Types { get; set; }
        public long? ItemId { get; set; }
        public int ItemVolume { get; set; }
        public bool IsUse { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
