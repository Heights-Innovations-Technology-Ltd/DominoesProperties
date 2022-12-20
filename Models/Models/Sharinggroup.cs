#nullable disable

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models.Models
{
    public partial class Sharinggroup
    {
        public Sharinggroup()
        {
            Sharingentries = new HashSet<Sharingentry>();
        }

        public string UniqueId { get; set; }
        public string Alias { get; set; }
        public long PropertyId { get; set; }
        public int MaxCount { get; set; }
        public string CustomerUniqueId { get; set; }
        public DateTime Date { get; set; }
        public bool? IsClosed { get; set; }
        public int PercentageSubscribed { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsInvested { get; set; } = false;

        [IgnoreDataMember] public virtual Property Property { get; set; }

        public virtual ICollection<Sharingentry> Sharingentries { get; set; }
    }
}