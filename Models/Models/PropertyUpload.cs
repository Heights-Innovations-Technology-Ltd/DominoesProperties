using System;
using System.Collections.Generic;

#nullable disable

namespace Models.Models
{
    public partial class PropertyUpload
    {
        public long Id { get; set; }
        public string PropertyId { get; set; }
        public string Url { get; set; }
        public string ImageName { get; set; }
        public DateTime? DateUploaded { get; set; }
    }
}
