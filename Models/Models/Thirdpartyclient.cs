#nullable disable

using System;

namespace Models.Models
{
    public partial class Thirdpartyclient
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public int ClientId { get; set; }
        public string ApiKey { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}