using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectAPI.DataAccess.Primitives.Abstractions;

namespace ProjectAPI.DataAccess.Primitives
{
	public class Category : Auditable
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Category name is required")]
		public string Name { get; set; }
		public string? Description { get; set; }

		public List<Product> Products { get; set; }

		//product specification data
		public List<string> Specifications { get; set; } = new List<string>();
	}
}