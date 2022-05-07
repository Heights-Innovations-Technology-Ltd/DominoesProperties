using System;
using System.Collections.Generic;

#nullable disable

namespace Models.Models
{
    public partial class Transaction
    {
        public string TransactionRef { get; set; }
        public decimal Amount { get; set; }
        public long CustomerId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Module { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public string Channel { get; set; }
        public string RequestPayload { get; set; }
        public string ResponsePayload { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
