#nullable disable

using System;

namespace Models.Models
{
    public partial class Thirdpartycustomer
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Channel { get; set; }
        public DateTime DateRegistered { get; set; } = DateTime.Now;
    }
}