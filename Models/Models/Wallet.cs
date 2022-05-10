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
        public decimal? Limit { get; set; } = new decimal(0);
        public decimal? Balance { get; set; } = new decimal(0);
        public DateTime? LastTransactionDate { get; set; }
        public decimal LastTransactionAmount { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
