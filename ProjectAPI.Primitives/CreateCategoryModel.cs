using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Primitives
{
	/// <summary>
	/// Model class for the Product Category entity which is used to receive the category information when adding one.
	/// </summary>
	public class CreateCategoryModel
	{
		/// <summary>
		/// Gets and sets the displayed or received category name.
		/// </summary>
		[Required(ErrorMessage = "Category name is required")]
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the displayed or received category description.
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Gets and sets the displayed or received list of specifications for the products in the category.
		/// </summary>
		public List<string> Specifications { get; set; } = new List<string>();
	}
}