using System;
using System.Collections.Generic;

#nullable disable

namespace Models.Models
{
    public partial class PaystackPayment
    {
        public long Id { get; set; }
        public string AccessCode { get; set; }
        public decimal Amount { get; set; }
        public string Channel { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string Type { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
        public string TransactionRef { get; set; }
        public string Payload { get; set; }
        public string PaymentModule { get; set; }
    }
}
