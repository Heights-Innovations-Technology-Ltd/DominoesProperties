using System.ComponentModel.DataAnnotations;
using DominoesProperties.Enums;
using Microsoft.AspNetCore.Http;

namespace DominoesProperties.Models
{
    public class PropertyFileUpload
    {
        [Required(ErrorMessage = "A valid file is required")]
        [MaxLength(2 * 1024 * 1024, ErrorMessage = "Upload size cannot exceed 2MB")]
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }
        public UploadType UploadType { get; set; } = UploadType.PICTURE;

    }
}

