using System.Collections.Generic;
using System;

namespace ProjectAPI.Models
{
	public class Product : ISoftDelete
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }

		public int CategoryId { get; set; }
		public Category Category { get; set; }

		public DateTime? DeletedOn { get; set; }

		//product specification data
		Dictionary<string, string> SpecificationData { get; set; } = new();
	}
}

