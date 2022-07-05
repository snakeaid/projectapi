using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: good xml comments
namespace ProjectAPI.Primitives
{
	/// <summary>
	/// Model class for the Product entity which is used for user interaction
	/// </summary>
	public class ProductModel
	{
		/// <summary>
		/// Displayed product unique identifier
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Displayed name of the product
		/// </summary>
		[Required(ErrorMessage = "Product name is required")]
		public string Name { get; set; }

		/// <summary>
		/// Displayed description of the product
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Displayed category id of the product
		/// </summary>
		public int CategoryId { get; set; }

		/// <summary>
		/// Displayed date of creation of the product
		/// </summary>
		public DateTimeOffset DateCreated { get; set; }

		/// <summary>
		/// Displayed date of the last update of the product
		/// </summary>
		public DateTimeOffset? DateUpdated { get; set; }

		/// <summary>
		/// Displayed list of product specifications
		/// </summary>
		public Dictionary<string, string> SpecificationData { get; set; } = new Dictionary<string, string>();
	}
}

