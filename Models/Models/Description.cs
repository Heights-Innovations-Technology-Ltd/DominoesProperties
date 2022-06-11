using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Models.Models
{
    public partial class Description
    {
        public Description()
        {
            Properties = new HashSet<Property>();
        }

        public long Id { get; set; }
        public long PropertyId { get; set; }
        public int? Bathroom { get; set; }
        public int? Toilet { get; set; }
        public int? FloorLevel { get; set; }
        public int? Bedroom { get; set; }
        public string LandSize { get; set; }
        public bool? AirConditioned { get; set; }
        public bool? Refrigerator { get; set; }
        public bool? Parking { get; set; }
        public bool? SwimmingPool { get; set; }
        public bool? Laundry { get; set; }
        public bool? Gym { get; set; }
        public bool? SecurityGuard { get; set; }
        public bool? Fireplace { get; set; }
        public bool? Basement { get; set; }

        [NotMapped]
        public virtual Property Property { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
    }
}
