using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class SharingGroup : InvestmentSharing
    {
        [Required(ErrorMessage = "Alias is required for every new group")]
        [MaxLength(20, ErrorMessage = "Alias should not be more than 20 charcters long")]
        public string Alias { get; set; }
    }
}

