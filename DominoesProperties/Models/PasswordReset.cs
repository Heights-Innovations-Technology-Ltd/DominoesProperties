using System;
using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class PasswordReset
    {
        [Required(ErrorMessage = "Password is required")]
        [MaxLength(50)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password does not match specified pattern")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password is required")]
        [MaxLength(50)]
        [Compare("Password", ErrorMessage = "Password and Confirm password does not match")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password does not match specified pattern")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Validation token is not supplied or incorrect")]
        [Display(Description = "Supply old password on change password and token should be supplied on reset password")]
        public string Token { get; set; }
    }
}
