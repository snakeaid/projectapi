using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Primitives
{
	/// <summary>
	/// Model class for the Product entity which is used to receive the product information when updating it.
	/// </summary>
	public class UpdateProductModel
	{
		/// <summary>
		/// Gets and sets the received product name.
		/// </summary>
		[Required(ErrorMessage = "Product name is required")]
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the received product description.
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Gets and sets the received category id of the product.
		/// </summary>
		public int CategoryId { get; set; }

		/// <summary>
		/// Gets and sets the received list of product specifications.
		/// </summary>
		public Dictionary<string, string> SpecificationData { get; set; } = new Dictionary<string, string>();
	}
}

