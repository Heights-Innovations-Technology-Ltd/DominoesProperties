using System;
using System.Collections.Generic;

#nullable disable

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
        public bool? IsDeleted { get; set; }
        public string UniqueNumber { get; set; }

        public virtual Admin CreatedByNavigation { get; }
    }
}
