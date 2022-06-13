﻿using System;
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

        //[IgnoreDataMember]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateCreated { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Property> Properties { get; set; }
    }
}
