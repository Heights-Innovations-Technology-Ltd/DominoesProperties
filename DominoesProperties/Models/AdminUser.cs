using System;
using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class AdminUser
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Not a valid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MaxLength(50)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password does not match specified pattern")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Role must be assigned to user")]
        public int RoleFk { get; set; }
    }
}
