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
    }
}