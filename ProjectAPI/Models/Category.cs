using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Models
{
	public class Category : ISoftDelete
	{
		public int Id { get; set; }
		[Required(ErrorMessage="Category name is required")]
		public string Name { get; set; }
		public string? Description { get; set; }

		public List<Product> Products { get; set; } = new();

		public DateTime? DeletedOn { get; set; }

		//product specification data
		List<string> Specifications { get; set; } = new();
	}
}

