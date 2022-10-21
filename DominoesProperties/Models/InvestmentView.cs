using System;
using System.Runtime.Serialization;

namespace DominoesProperties.Models
{
    public class InvestmentView
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long PropertyId { get; set; }
        public int Units { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public decimal? Yield { get; set; }
        public string PaymentType { get; set; }
        public decimal? YearlyInterestAmount { get; set; }
        public string TransactionRef { get; set; }
        public string Status { get; set; }
        [IgnoreDataMember]
        public decimal UnitPrice { get; set; }
        public string Customer { get; set; }
        public string Property { get; set; }
        public string Data { get; set; }
    }
}

