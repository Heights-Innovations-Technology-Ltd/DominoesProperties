using System.ComponentModel.DataAnnotations;
using DominoesProperties.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DominoesProperties.Models
{
    public class InvestmentNew
    {
        [Required(ErrorMessage ="Property Id is required")]
        public string PropertyUniqueId { get; set; }
        [Range(1, 100)]
        [Required(ErrorMessage ="Number of units to be bought is required")]
        public int Units { get; set; }
        public Channel Channel { get; set; }
    }
}