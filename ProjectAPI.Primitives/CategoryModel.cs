using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: good xml comments
namespace ProjectAPI.Primitives
{
	/// <summary>
    /// Model class for the Product Category entity which is used for user interactions
    /// </summary>
	public class CategoryModel
	{
		/// <summary>
		/// Displayed category unique identifier
		/// </summary>
		public int Id { get; set; }

		/// <summary>
        /// Displayed category name
        /// </summary>
		[Required(ErrorMessage = "Category name is required")]
		public string Name { get; set; }

		/// <summary>
		/// Displayed category description
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
        /// Displayed date of creation of the category
        /// </summary>
		public DateTimeOffset DateCreated { get; set; }

		/// <summary>
		/// Displayed date of the last update of the category
		/// </summary>
		public DateTimeOffset? DateUpdated { get; set; }

		/// <summary>
        /// Displayed list of specifications for the products in the category
        /// </summary>
		public List<string> Specifications { get; set; } = new List<string>();
	}
}