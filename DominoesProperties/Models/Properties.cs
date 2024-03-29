using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DominoesProperties.Models
{
    public class Properties
    {
        [JsonIgnore]
        public string UniqueId { get; set; }
        [Required(ErrorMessage ="Property name is required")]
        [MaxLength(250)]
        public string Name { get; set; }
        public string Location { get; set; }
        [Required(ErrorMessage ="Property type is required")]
        public int? Type { get; set; }
        [Required(ErrorMessage ="Total number of available units is required")]
        public int TotalUnits { get; set; }
        [Required(ErrorMessage ="Property price per unit is required")]
        public decimal UnitPrice { get; set; }
        [Required(ErrorMessage ="Current property status is required")]
        public string Status { get; set; }
        public int UnitSold { get; set; }
        public int UnitAvailable { get; set; }
        public DateTime? ClosingDate { get; set; }
        [Required(ErrorMessage ="Brief description of the property is required")]
        public PropertyDescription Description { get; set; }
        [Required(ErrorMessage = "Please specify a projected yield for the property")]
        public decimal? TargetYield { get; set; }
        [JsonIgnore]
        public decimal? ProjectedGrowth { get; set; }
        [JsonIgnore]
        public decimal? InterestRate { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Longitude { get; set; } = "";
        public string Latitude { get; set; } = "";
        public string TypeName { get; set; }
        [Required(ErrorMessage = "Property description summary is required")]
        [MinLength(50, ErrorMessage ="Description of this property cannot be less than 50 in length")]
        public string Summary { get; set; }
        [Required(ErrorMessage = "Property Bank name is required")]
        public string BankName { get; set; }
        [Required(ErrorMessage = "Property Account number is required")]
        public string AccountNumber { get; set; }
        public string VideoLink { get; set; }
        public object Data { get; set; }
        public bool AllowSharing { get; set; } = false;
        public int MinimumSharingPercentage { get; set; } = 0;

        [JsonIgnore]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        public bool? IsDeleted { get; set; }
    }
}