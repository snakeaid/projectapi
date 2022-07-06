using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectAPI.DataAccess.Primitives.Abstractions;

namespace ProjectAPI.DataAccess.Primitives
{
	/// <summary>
	/// Product Category entity class which implements <see cref="IAuditable"/>.
	/// </summary>
	public class Category : IAuditable
	{
		/// <summary>
		/// Gets and sets the unique identifier of the category.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets and sets the category name.
		/// </summary>
		[Required(ErrorMessage = "Category name is required")]
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the category description.
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Gets and sets the list of products in the category.
        /// </summary>
		public List<Product> Products { get; set; }

		/// <summary>
		/// Gets and sets the date of creation of the category.
		/// </summary>
		public DateTimeOffset DateCreated { get; set; }

		/// <summary>
		/// Gets and sets the date of the last update of the category.
		/// </summary>
		public DateTimeOffset? DateUpdated { get; set; }

		/// <summary>
		/// Gets and sets the date of deletion of the category.
		/// </summary>
		public DateTimeOffset? DateDeleted { get; set; }

		/// <summary>
		/// Gets and sets the list of specifications for the products in the category.
		/// </summary>
		public List<string> Specifications { get; set; } = new List<string>();
	}
}