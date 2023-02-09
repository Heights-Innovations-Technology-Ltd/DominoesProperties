using System.ComponentModel.DataAnnotations;
using DominoesProperties.Enums;

namespace DominoesProperties.Models
{
    public class InvestmentNew
    {
        [Required(ErrorMessage = "Property Id is required")]
        public string PropertyUniqueId { get; set; }

        [Required(ErrorMessage = "Number of units to be bought is required")]
        public int Units { get; set; }

        public Channel Channel { get; set; }
        public bool IsSharing { get; set; }
    }
}