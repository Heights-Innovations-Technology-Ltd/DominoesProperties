using System;
namespace DominoesProperties.Models
{
    public class CustomerTransaction
    {
        public string TransactionRef { get; set; }
        public decimal Amount { get; set; }
        public long CustomerId { get; set; }
        public DateTime? TransactionDate { get; set; } = DateTime.Now;
        public string Module { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public string Channel { get; set; }
        public string RequestPayload { get; set; }
        public string ResponsePayload { get; set; }
    }
}
