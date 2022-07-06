using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectAPI.DataAccess.Primitives.Abstractions;

namespace ProjectAPI.DataAccess.Primitives
{
	/// <summary>
	/// Product entity class which implements <see cref="IAuditable"/>.
	/// </summary>
	public class Product : IAuditable
	{
		/// <summary>
		/// Gets and sets the unique identifier of the product.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets and sets the product name.
		/// </summary>
		[Required(ErrorMessage = "Product name is required")]
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the product description.
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Gets and sets the category id of the product.
		/// </summary>
		public int CategoryId { get; set; }

		/// <summary>
		/// Gets and sets the category of the product.
		/// </summary>
		public Category Category { get; set; }

		/// <summary>
		/// Gets and sets the date of creation of the product.
		/// </summary>
		public DateTimeOffset DateCreated { get; set; }

		/// <summary>
		/// Gets and sets the date of the last update of the product.
		/// </summary>
		public DateTimeOffset? DateUpdated { get; set; }

		/// <summary>
		/// Gets and sets the date of the deletion of the product.
		/// </summary>
		public DateTimeOffset? DateDeleted { get; set; }

		/// <summary>
		/// Gets and sets the list of product specifications.
		/// </summary>
		public Dictionary<string, string> SpecificationData { get; set; } = new Dictionary<string, string>();
	}
}