using System;

#nullable disable

namespace Models.Models
{
    public partial class Sharingentry
    {
        public long Id { get; set; }
        public string GroupId { get; set; }
        public long CustomerId { get; set; }
        public bool? IsClosed { get; set; }
        public int PercentageShare { get; set; }
        public DateTime Date { get; set; }
        public string PaymentReference { get; set; }
        public bool IsReversed { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Sharinggroup Group { get; set; }
    }
}
