using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

#nullable disable

namespace Models.Models
{
    public partial class Admin
    {
        public Admin()
        {
            Enquiries = new HashSet<Enquiry>();
            Properties = new HashSet<Property>();
            PropertyImages = new HashSet<Propertyupload>();
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public int? RoleFk { get; set; }
        public bool? IsActive { get; set; } = false;
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        [IgnoreDataMember]
        public virtual Role RoleFkNavigation { get; set; }
        [IgnoreDataMember]
        public virtual ICollection<Property> Properties { get; set; }
        [IgnoreDataMember]
        [NotMapped]
        public virtual ICollection<Propertyupload> PropertyImages { get; set; }
        public virtual ICollection<Enquiry> Enquiries { get; set; }
    }
}
