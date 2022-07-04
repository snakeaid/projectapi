using System;

namespace ProjectAPI.DataAccess.Primitives.Abstractions
{
	public abstract class Auditable
	{
		public DateTimeOffset DateCreated { get; set; }
		public DateTimeOffset? DateUpdated { get; set; }
		public DateTimeOffset? DateDeleted { get; set; }
	}
}