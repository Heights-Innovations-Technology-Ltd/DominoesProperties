using System;
using System.Collections.Generic;

#nullable disable

namespace Models.Models
{
    public partial class PropertyImage
    {
        public long Id { get; set; }
        public long PropertyId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateUploaded { get; set; }
        public string UploadedBy { get; set; }

        public virtual Property Property { get; set; }
        public virtual Admin UploadedByNavigation { get; set; }
    }
}
