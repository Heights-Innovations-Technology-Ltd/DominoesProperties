using System;
using Newtonsoft.Json;

namespace Models.Models
{
    public partial class PropertyType
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public DateTime? DateCreated { get; set; }
    }
}
