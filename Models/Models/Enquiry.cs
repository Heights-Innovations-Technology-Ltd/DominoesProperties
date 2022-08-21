using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

#nullable disable

namespace Models.Models
{
    public partial class Enquiry
    {
        [IgnoreDataMember]
        public long Id { get; set; }
        [Required(ErrorMessage = "Customer reference is required")]
        public string CustomerUniqueReference { get; set; }
        [Required(ErrorMessage = "Property reference is required")]
        public string PropertyReference { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        [MaxLength(100, ErrorMessage = "Maximum length of 100 exceeded on subject")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Message cannot be empty")]
        public string Message { get; set; }
        public string Status { get; set; } = "NEW";
        public string ClosedBy { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Admin ClosedByNavigation { get; set; }
    }
}
