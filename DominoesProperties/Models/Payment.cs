using System;
using DominoesProperties.Enums;

namespace DominoesProperties.Models
{
    public class Payment
    {
        public decimal Amount { get; set; }
        public PaymentType Module { get; set; }
        public long InvestmentId { get; set; }
    }
}
