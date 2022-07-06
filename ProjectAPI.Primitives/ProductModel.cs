using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Primitives
{
	/// <summary>
	/// Model class for the Product entity which is used for user interaction.
	/// </summary>
	public class ProductModel
	{
		/// <summary>
		/// Gets and sets the displayed unique identifier of the product.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets and sets the displayed or received product name.
		/// </summary>
		[Required(ErrorMessage = "Product name is required")]
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the displayed or received product description.
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Gets and sets the displayed or received category id of the product.
		/// </summary>
		public int CategoryId { get; set; }

		/// <summary>
		/// Gets and sets the displayed date of creation of the product.
		/// </summary>
		public DateTimeOffset DateCreated { get; set; }

		/// <summary>
		/// Gets and sets the displayed date of the last update of the product.
		/// </summary>
		public DateTimeOffset? DateUpdated { get; set; }

		/// <summary>
		/// Gets and sets the displayed or received list of product specifications.
		/// </summary>
		public Dictionary<string, string> SpecificationData { get; set; } = new Dictionary<string, string>();
	}
}

