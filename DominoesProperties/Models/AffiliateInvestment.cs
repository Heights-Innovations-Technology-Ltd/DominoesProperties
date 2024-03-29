using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class AffiliateInvestment
    {
        [Required(ErrorMessage = "Email is required")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Not a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The unique identifier of the property is required")]
        public string PropertyId { get; set; }

        [Required(ErrorMessage = "No of investment units is required")]
        public int Units { get; set; }

        [Required(ErrorMessage = "Amount paid is required")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please supply the payment transaction reference")]
        public string TransactionRef { get; set; }
    }
}