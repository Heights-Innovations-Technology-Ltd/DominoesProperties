using System;
using System.Collections.Generic;

#nullable disable

namespace Models.Models
{
    public partial class Admin
    {
        public Admin()
        {
            Properties = new HashSet<Property>();
            PropertyImages = new HashSet<PropertyImage>();
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public int? RoleFk { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DateCreated { get; set; }

        public virtual Role RoleFkNavigation { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<PropertyImage> PropertyImages { get; set; }
    }
}
