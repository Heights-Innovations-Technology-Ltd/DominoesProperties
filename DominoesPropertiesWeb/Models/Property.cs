﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DominoesPropertiesWeb.Models
{
    public class Property
    {
        [JsonIgnore]
        public string UniqueId { get; set; }
        [Required(ErrorMessage = "Property name is required")]
        [MaxLength(250)]
        public string Name { get; set; }
        public string Location { get; set; }
        [Required(ErrorMessage = "Property type is required")]
        public int? Type { get; set; }
        [Required(ErrorMessage = "Total number of available units is required")]
        public int TotalUnits { get; set; }
        [Required(ErrorMessage = "Property price per unit is required")]
        public decimal UnitPrice { get; set; }
        [Required(ErrorMessage = "Current property status is required")]
        public string Status { get; set; }
        public int? UnitSold { get; set; }
        public int? UnitAvailable { get; set; }
        public DateTime? ClosingDate { get; set; }
        [Required(ErrorMessage = "Brief description of the property is required")]
        public PropertyDescription Description { get; set; }
        public decimal? TargetYield { get; set; }
        public decimal? ProjectedGrowth { get; set; }
        [Required(ErrorMessage = "Property yield is required")]
        public decimal? InterestRate { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        [JsonIgnore]
        public string CreatedBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
