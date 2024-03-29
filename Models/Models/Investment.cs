﻿using System;
using System.Runtime.Serialization;

#nullable disable

namespace Models.Models
{
    public partial class Investment
    {
        [IgnoreDataMember]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long PropertyId { get; set; }
        public int Units { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public decimal? Yield { get; set; }
        [IgnoreDataMember]
        public string PaymentType { get; set; }
        public decimal? YearlyInterestAmount { get; set; }
        public string TransactionRef { get; set; }
        public string Status { get; set; }
        public decimal UnitPrice { get; set; }


        public virtual Customer Customer { get; set; }
        public virtual Property Property { get; set; }
        public virtual Transaction TransactionRefNavigation { get; set; }
    }
}
