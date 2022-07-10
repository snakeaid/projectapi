using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Primitives
{
	/// <summary>
	/// Model class for the Product Category entity which is used to receive the category information when updating it.
	/// </summary>
	public class UpdateCategoryModel
	{
		/// <summary>
		/// Gets and sets the received category name.
		/// </summary>
		[Required(ErrorMessage = "Category name is required")]
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the received category description.
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Gets and sets the received list of specifications for the products in the category.
		/// </summary>
		public List<string> Specifications { get; set; } = new List<string>();
	}
}