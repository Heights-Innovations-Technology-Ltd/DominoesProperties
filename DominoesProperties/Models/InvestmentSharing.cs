using System;
using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class InvestmentSharing
    {
        [Required(ErrorMessage = "Property reference is required")]
        public string PropertyUniqueId { get; set; }
        [Required(ErrorMessage = "Percentage of the unit is required")]
        public int PercentageShare { get; set; }
        public string SharingGroupId { get; set; }
    }
}

