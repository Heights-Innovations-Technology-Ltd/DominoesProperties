using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class AdminInvestment
    {
        [Required(ErrorMessage = "Property Id is required")]
        public string PropertyUniqueId { get; set; }

        [Required(ErrorMessage = "Supply email of an existing customer")]
        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "Number of units to be bought is required")]
        public int Units { get; set; }

        public bool IsSharing { get; set; }
    }
}