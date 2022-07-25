using System;
using DominoesProperties.Enums;

namespace DominoesProperties.Models
{
    public class PropertyFileUpload
    {
        public string Url { get; set; }
        public string ImageName { get; set; }
        public DateTime? DateUploaded { get; set; }
        public UploadType UploadType { get; set; }
    }
}

