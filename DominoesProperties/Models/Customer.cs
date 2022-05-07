using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class Customer : Profile
    {
        [Required(ErrorMessage = "Password is required")]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [MaxLength(50)]
        public string ConfirmPassword { get; set; }
    }
}
