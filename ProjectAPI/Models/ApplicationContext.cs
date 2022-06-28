using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace ProjectAPI.Models
{
	public class ApplicationContext : DbContext
	{
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public ApplicationContext(DbContextOptions<ApplicationContext> options)
			: base(options)
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ISoftDelete>().HasQueryFilter(e => e.DeletedOn != null);
		}

		public override int SaveChanges()
		{
			HandleBookDelete();
			return base.SaveChanges();
		}

		private void HandleBookDelete()
		{
			var entities = ChangeTracker.Entries()
								.Where(e => e.State == EntityState.Deleted);
			foreach (var entity in entities)
			{
				if (entity.Entity is ISoftDelete)
				{
					entity.State = EntityState.Modified;
					var e = entity.Entity as ISoftDelete;
					e.DeletedOn = DateTime.Now;
				}
			}

		}
	}
}
