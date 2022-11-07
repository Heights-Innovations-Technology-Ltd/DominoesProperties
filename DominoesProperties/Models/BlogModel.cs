using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominoesProperties.Models
{
    public class BlogModel
    {
        [NotMapped]
        public string UniqueNumber { get; set; }

        [Required (ErrorMessage = "Blog title is required")]
        [Column(TypeName = "VARCHAR(200)")]
        public string BlogTitle { get; set; }

        [Required(ErrorMessage = "Blog Content is required")]
        [Column(TypeName = "VARCHAR(MAX)")]
        public string BlogContent { get; set; }

        [Column(TypeName = "VARCHAR(500)")]
        public string BlogTags { get; set; }

        [Column(TypeName = "VARCHAR(500)")]
        public string BlogImage { get; set; }

        public string CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }
    }
}
