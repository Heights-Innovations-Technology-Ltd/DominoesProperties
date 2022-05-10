
using System;

namespace DominoesProperties.Models
{
    public class UpdateProperty
    {
        public string Location { get; set; }
        public int? Type { get; set; }
        public int TotalUnits { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? ClosingDate { get; set; }
        public decimal? TargetYield { get; set; }
        public decimal? ProjectedGrowth { get; set; }
        public decimal? InterestRate { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}