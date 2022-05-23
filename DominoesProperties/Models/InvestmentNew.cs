using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DominoesProperties.Models
{
    public class InvestmentNew
    {
        [Required(ErrorMessage ="Property Id is required")]
        public string PropertyUniqueId { get; set; }
        [Required(ErrorMessage ="Number of units to be bought is required")]
        [MinLength(1)]
        public int Units { get; set; }
    }
}