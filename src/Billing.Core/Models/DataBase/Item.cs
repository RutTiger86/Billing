﻿namespace Billing.Core.Models.DataBase
{
    public class Item
    {
        public long Id { get; set; }
        public required string ItemName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
