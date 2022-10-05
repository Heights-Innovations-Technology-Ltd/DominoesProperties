using System;

#nullable disable

namespace Models.Models
{
    public partial class Newsletter
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;
    }
}
