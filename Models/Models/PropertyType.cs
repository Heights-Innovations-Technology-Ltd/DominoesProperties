using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable

namespace Models.Models
{
    public partial class PropertyType
    {
        public PropertyType()
        {
            Properties = new HashSet<Property>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;

        [IgnoreDataMember]
        public virtual ICollection<Property> Properties { get; set; }
    }
}
