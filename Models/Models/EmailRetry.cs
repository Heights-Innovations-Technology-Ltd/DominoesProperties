using System;

#nullable disable

namespace Models.Models
{
    public partial class EmailRetry
    {
        public long Id { get; set; }
        public string Category { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string RecipientName { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public string StatusCode { get; set; }
        public int RetryCount { get; set; }
    }
}
