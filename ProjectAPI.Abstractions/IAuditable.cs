using System;

namespace ProjectAPI.DataAccess.Primitives.Abstractions
{
	public interface IAuditable
	{
		public DateTimeOffset DateCreated { get; set; }
		public DateTimeOffset? DateUpdated { get; set; }
		public DateTimeOffset? DateDeleted { get; set; }
	}
}