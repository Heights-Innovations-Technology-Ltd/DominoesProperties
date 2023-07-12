using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class AffiliateCustomer
    {
        [Required(ErrorMessage = "Firstname is required")]
        [MaxLength(50, ErrorMessage = "Firstname cannot be more than 50 characters")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Characters are not allowed.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        [MaxLength(50, ErrorMessage = "Lastname cannot be more than 50 characters")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Characters are not allowed.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Not a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(14)]
        public string Phone { get; set; }

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