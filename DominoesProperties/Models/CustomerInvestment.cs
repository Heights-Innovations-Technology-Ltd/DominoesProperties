using System;
namespace DominoesProperties.Models
{
    public class CustomerInvestment
    {
        public long CustomerId { get; set; }
        public long PropertyId { get; set; }
        public int Units { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal? Yield { get; set; }
        public string PaymentType { get; set; }
        public decimal? YearlyInterestAmount { get; set; }
    }
}
