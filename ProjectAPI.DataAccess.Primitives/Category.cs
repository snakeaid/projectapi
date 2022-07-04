using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectAPI.DataAccess.Primitives.Abstractions;

namespace ProjectAPI.DataAccess.Primitives
{
	public class Category : IAuditable
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Category name is required")]
		public string Name { get; set; }
		public string? Description { get; set; }

		public List<Product> Products { get; set; }

		public DateTimeOffset DateCreated { get; set; }
		public DateTimeOffset? DateUpdated { get; set; }
		public DateTimeOffset? DateDeleted { get; set; }

		//product specification data
		public List<string> Specifications { get; set; } = new List<string>();
	}
}