using Billing.Core.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Models
{
    public class ProductInfo
    {        
        public long Id { get; set; }
        public required string ProductKey { get; set; }
        public required string ProductName { get; set; }
        public bool IsUse { get; set; }
        public required List<ProductItemInfo> Items { get; set; }
    }
}
