﻿using Billing.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Models.DataBase
{
    public class Ledger
    {
        public long Id { get; set; }
        public long AccountId {get; set; }
        public PointType Type {get; set; }
        public long Balance {get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
