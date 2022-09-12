using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable

namespace Models.Models
{
    public partial class Property
    {
        public Property()
        {
            Investments = new HashSet<Investment>();
            PropertyUploads = new HashSet<Propertyupload>();
            Sharinggroups = new HashSet<Sharinggroup>();
        }

        public long Id { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Type { get; set; }
        public int TotalUnits { get; set; } = 0;
        public decimal UnitPrice { get; set; } = 0;
        public decimal TotalPrice { get; set; } = 0;
        public string Status { get; set; } = "ONGOING_CONSTRUCTION";
        public int UnitSold { get; set; } = 0;
        public int UnitAvailable { get; set; } = 0;
        public DateTime? ClosingDate { get; set; }
        public decimal TargetYield { get; set; } = 0;
        public decimal ProjectedGrowth { get; set; } = 0;
        public decimal InterestRate { get; set; } = 0;
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string CreatedBy { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public string Summary { get; set; }
        public string Account { get; set; }
        public string Bank { get; set; }
        public int MaxUnitPerCustomer { get; set; } = 1000;
        public string VideoLink { get; set; }
        public bool? AllowSharing { get; set; }
        public int? MinimumSharingPercentage { get; set; }

        public virtual Admin CreatedByNavigation { get; set; }
        public virtual PropertyType TypeNavigation { get; set; }
        [IgnoreDataMember]
        public virtual ICollection<Investment> Investments { get; set; }
        public virtual ICollection<Propertyupload> PropertyUploads { get; set; }
        [IgnoreDataMember]
        public virtual ICollection<Sharinggroup> Sharinggroups { get; set; }
    }
}
