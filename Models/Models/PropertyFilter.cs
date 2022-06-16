using System;
namespace Models.Models
{
	public class PropertyFilter
	{
		public string? Category { get; set; }
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
		public int? Bedroom { get; set; }
		public int? Bathroom { get; set; }
		public int? Floor { get; set; }
		public int? Toilet { get; set; }
		public bool? Parking { get; set; }
		public bool? SwimmingPool { get; set; }
		public bool? Refridgerator { get; set; }
		public bool? Gym { get; set; }
		public bool? Laundry { get; set; }
		public bool? Fireplace { get; set; }
		public bool? AirConditioned { get; set; }
		public bool? Basement { get; set; }
		public bool? SecurityGuard { get; set; }
	}
}

