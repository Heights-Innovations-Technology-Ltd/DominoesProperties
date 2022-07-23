using System.Text.Json.Serialization;
using DominoesProperties.Enums;

namespace DominoesProperties.Models
{
    public class Payment
    {
        public decimal Amount { get; set; }
        public PaymentType Module { get; set; }
        [JsonIgnore]
        public long InvestmentId { get; set; }
        [JsonIgnore]
        public string Callback { get; set; }
    }
}
