using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Models
{
	public class Product : Auditable
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Product name is required")]
		public string Name { get; set; }
		public string? Description { get; set; }

		public int CategoryId { get; set; }
		public Category Category { get; set; }

		//product specification data
		Dictionary<string, string> SpecificationData { get; set; }
	}
}

