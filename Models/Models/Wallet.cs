using System;
using System.Collections.Generic;

#nullable disable

namespace Models.Models
{
    public partial class Wallet
    {
        public int Id { get; set; }
        public string WalletNo { get; set; }
        public long CustomerId { get; set; }
        public decimal? Limit { get; set; } = (decimal)0.00;
        public decimal? Balance { get; set; } = (decimal)0.00;
        public DateTime? LastTransactionDate { get; set; }
        public decimal LastTransactionAmount { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public virtual Customer Customer { get; set; }
    }
}
