using System.ComponentModel.DataAnnotations;
using DominoesProperties.Enums;

namespace DominoesProperties.Models
{
    public class InvestmentNew : SharingGroup
    {
        [Required(ErrorMessage ="Property Id is required")]
        public string PropertyUniqueId { get; set; }
        [Range(1, 100)]
        [Required(ErrorMessage ="Number of units to be bought is required")]
        public int Units { get; set; }
        public Channel PaymentChannel { get; set; }
        public bool IsSharing { get; set; }
    }
}