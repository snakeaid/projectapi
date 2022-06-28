using System;
namespace ProjectAPI.Models
{
	public interface ISoftDelete
	{
		public DateTime? DeletedOn { get; set; }
	}
}

