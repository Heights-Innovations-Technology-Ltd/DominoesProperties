#nullable disable

using System;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string PaystackRef { get; set; }

        [Column("Charges", TypeName = "decimal(10,2)")]
        public decimal Charges { get; set; }
    }
}