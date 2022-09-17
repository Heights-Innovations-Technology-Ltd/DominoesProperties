using System;
using System.Runtime.Serialization;

#nullable disable

namespace Models.Models
{
    public partial class Propertyupload
    {
        public long? Id { get; set; }
        public long? PropertyId { get; set; }
        public string Url { get; set; }
        public string ImageName { get; set; }
        public DateTime? DateUploaded { get; set; }
        public string UploadType { get; set; }
        public string AdminEmail { get; set; }

        [IgnoreDataMember]
        public virtual Admin AdminEmailNavigation { get; set; }
        [IgnoreDataMember]
        public virtual Property Property { get; set; }
    }
}
