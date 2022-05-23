using System;
using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class ChangePassword
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
    }
}
