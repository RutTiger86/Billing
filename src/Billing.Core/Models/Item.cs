using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Models
{
    public class Item
    {
        public long Id { get; set; }
        public required string ItemName { get; set; }
    }
}
