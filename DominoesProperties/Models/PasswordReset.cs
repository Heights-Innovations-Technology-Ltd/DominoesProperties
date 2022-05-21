using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace DominoesProperties.Models
{
    [SwaggerSchema(Description = "For use on Change Password and Reset Password operations", Required = new string[] { "Password", "ConfirmPassword", "Token"})]
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
        ///<summary>
        ///This parameter takes the OLD PASSWORD for endpoint, CHANGE PASSWORD and it takes TOKEN for endpoint, RESET PASSWORD
        ///</summary>
        [Required(ErrorMessage = "Validation token is not supplied or incorrect")]
        public string Token { get; set; }
    }
}
