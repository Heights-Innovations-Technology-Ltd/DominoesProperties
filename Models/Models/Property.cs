using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Models.Models
{
    public partial class Property
    {
        public Property()
        {
            Investments = new HashSet<Investment>();
            PropertyUploads = new HashSet<PropertyUpload>();
        }

        public long Id { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int? Type { get; set; }
        public int TotalUnits { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Status { get; set; }
        public int? UnitSold { get; set; }
        public int? UnitAvailable { get; set; }
        public DateTime? ClosingDate { get; set; }
        public decimal? TargetYield { get; set; }
        public decimal? ProjectedGrowth { get; set; }
        public decimal? InterestRate { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string CreatedBy { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Admin CreatedByNavigation { get; set; }
        public virtual PropertyType TypeNavigation { get; set; }
        public virtual ICollection<Investment> Investments { get; set; }
        public virtual ICollection<PropertyUpload> PropertyUploads { get; set; }
    }
}
