using System;

namespace ProjectAPI.DataAccess.Primitives.Abstractions
{
	/// <summary>
    /// Describes all entities which are auditable for creation/modification and soft-deletable.
    /// </summary>
	public interface IAuditable
	{
		/// <summary>
		/// Gets and sets the date of creation of the entity.
		/// </summary>
		public DateTimeOffset DateCreated { get; set; }

		/// <summary>
		/// Gets and sets the date of the last update of the enitity.
		/// </summary>
		public DateTimeOffset? DateUpdated { get; set; }

		/// <summary>
		/// Gets and sets the date of the deletion of the entity.
		/// </summary>
		public DateTimeOffset? DateDeleted { get; set; }
	}
}