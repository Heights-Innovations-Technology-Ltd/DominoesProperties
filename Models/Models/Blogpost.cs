#nullable disable

using System;

namespace Models.Models
{
    public partial class Blogpost
    {
        public long Id { get; set; }
        public string BlogTitle { get; set; }
        public string BlogContent { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string BlogTags { get; set; }
        public string BlogImage { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public string UniqueNumber { get; set; }

        public virtual Admin CreatedByNavigation { get; }
    }
}