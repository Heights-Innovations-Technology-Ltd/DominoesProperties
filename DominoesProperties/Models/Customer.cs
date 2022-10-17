using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class CustomerReq
    {
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password cannot be less than 8 characters")]
        [MaxLength(16, ErrorMessage = "Password cannot be more than 16 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password does not match specified pattern. Must contain atleast one uppercase, one lowercase, one number and a special character")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password is required")]
        [MinLength(8, ErrorMessage = "Password cannot be less than 8 characters")]
        [MaxLength(16, ErrorMessage = "Password cannot be more than 16 characters")]
        [Compare("Password", ErrorMessage = "Password and Confirm password does not match")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Confirm password does not match specified pattern. Must contain atleast one uppercase, one lowercase, one number and a special character")]
        public string ConfirmPassword { get; set; }
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
        [EmailAddress(ErrorMessage ="Not a valid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(14)]
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AccountNumber { get; set; }
    }
}
