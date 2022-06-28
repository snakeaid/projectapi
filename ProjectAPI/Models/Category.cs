using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Models
{
	public class Category : Auditable
	{
		public int Id { get; set; }
		[Required(ErrorMessage="Category name is required")]
		public string Name { get; set; }
		public string? Description { get; set; }

		public List<Product> Products { get; set; }

		//product specification data
		List<string> Specifications { get; set; }
	}
}

