﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAPI.Models
{
	public class Product : Auditable
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Product name is required")]
		public string Name { get; set; }
		public string? Description { get; set; }

		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]
		public Category Category { get; set; }

		//product specification data
		public Dictionary<string, string> SpecificationData { get; set; } = new Dictionary<string, string>();
	}
}