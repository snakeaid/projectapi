using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Models
{
	public class CategoryDTO
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Category name is required")]
		public string Name { get; set; }
		public string? Description { get; set; }

//		public List<ProductDTO> Products { get; set; }

		public DateTimeOffset DateCreated { get; set; }
		public DateTimeOffset? DateUpdated { get; set; }

		//product specification data
		public List<string> Specifications { get; set; } = new List<string>();
	}
}