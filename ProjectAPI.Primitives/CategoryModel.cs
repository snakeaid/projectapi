using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Primitives
{
	/// <summary>
    /// Model class for the Product Category entity which is used to display the category information.
    /// </summary>
	public class CategoryModel
	{
		/// <summary>
		/// Gets and sets the displayed unique identifier of the category.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets and sets the displayed category name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the displayed category description.
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Gets and sets the displayed date of creation of the category.
		/// </summary>
		public DateTimeOffset DateCreated { get; set; }

		/// <summary>
		/// Gets and sets the displayed date of the last update of the category.
		/// </summary>
		public DateTimeOffset? DateUpdated { get; set; }

		/// <summary>
		/// Gets and sets the displayed list of specifications for the products in the category.
		/// </summary>
		public List<string> Specifications { get; set; } = new List<string>();
	}
}