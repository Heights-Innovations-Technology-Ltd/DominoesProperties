#nullable disable

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Investments = new HashSet<Investment>();
            OfflineInvestments = new HashSet<OfflineInvestment>();
            Transactions = new HashSet<Transaction>();
            Sharingentries = new HashSet<Sharingentry>();
        }

        public long Id { get; set; }
        public string UniqueRef { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool? IsActive { get; set; } = false;
        public bool? IsVerified { get; set; } = false;
        public string Password { get; set; }
        public bool? IsSubscribed { get; set; } = false;
        public string AccountNumber { get; set; }
        public bool? IsAccountVerified { get; set; } = false;
        public bool? IsDeleted { get; set; } = false;
        public string PassportUrl { get; set; }
        public string BankName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateRegistered { get; set; } = DateTime.Now;
        public DateTime? NextSubscriptionDate { get; set; }
        public DateTime? PrevSubscriptionDate { get; set; }

        public virtual Wallet Wallet { get; set; }
        public virtual ICollection<Investment> Investments { get; set; }
        public virtual ICollection<OfflineInvestment> OfflineInvestments { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Sharingentry> Sharingentries { get; set; }

        [IgnoreDataMember] public virtual ICollection<Thirdpartyinvestment> Thirdpartyinvestments { get; set; }
    }
}