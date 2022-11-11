﻿#nullable disable

using System;

namespace Models.Models
{
    public partial class OfflineInvestment
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long PropertyId { get; set; }
        public int Units { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentRef { get; set; }
        public string Status { get; set; }
        public decimal? UnitPrice { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Property Property { get; set; }
    }
}