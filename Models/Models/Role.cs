using System;
using System.Collections.Generic;

#nullable disable

namespace Models.Models
{
    public partial class Role
    {
        public Role()
        {
            Admins = new HashSet<Admin>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }
        public string CreatedBy { get; set; }
        public string Privilege { get; set; }
        public string Page { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;

        public virtual ICollection<Admin> Admins { get; set; }
    }
}
