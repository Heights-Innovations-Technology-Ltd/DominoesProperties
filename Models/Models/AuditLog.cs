using System;
using System.Collections.Generic;

#nullable disable

namespace Models.Models
{
    public partial class AuditLog
    {
        public long Id { get; set; }
        public string RequestPayload { get; set; }
        public string ResponsePayload { get; set; }
        public long? CustomerId { get; set; }
        public DateTime? Date { get; set; }
        public long? AdminId { get; set; }
    }
}
