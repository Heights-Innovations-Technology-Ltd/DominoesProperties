using System;

#nullable disable

namespace DominoesPropertiesWeb.Models
{
    public partial class PropertyUpload
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string ImageName { get; set; }
        public DateTime? DateUploaded { get; set; }
        public string UploadType { get; set; }
    }
}
