using Billing.Core.Enums;
using Billing.Core.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Models
{
    public class ProductItemInfo
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public ProductTypes Types { get; set; }
        public long ItemId { get; set; }
        public int ItemVolume { get; set; }
        public required string ItemName { get; set; }
        public PointType PointType { get; set; }
        public bool IsUse { get; set; }
    }
}
