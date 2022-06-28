using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

		//реализация soft delete
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ISoftDelete>().HasQueryFilter(e => e.DeletedOn != null);
		}

		public override int SaveChanges()
		{
			HandleSoftDelete();
			return base.SaveChanges();
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken=default)
		{
			HandleSoftDelete();
			return await base.SaveChangesAsync();
		}

		private void HandleSoftDelete()
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
