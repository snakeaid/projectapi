using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectAPI.Models
{
	public class CatalogContext : DbContext
	{
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public CatalogContext(DbContextOptions<CatalogContext> options)
			: base(options)
		{
			///Database.EnsureDeleted();
			Database.EnsureCreated();
		}

        //реализация (не)вывода soft delete
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(Auditable).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "p");
                    var deletedCheck = Expression.Lambda(Expression.Equal(Expression.Property(parameter, "DateDeleted"), Expression.Constant(null)), parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(deletedCheck);
                }
            }
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
                        .Property(p => p.SpecificationData)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
			modelBuilder.Entity<Category>()
						.Property(c => c.Specifications)
						.HasConversion(
							v => JsonConvert.SerializeObject(v),
							v => JsonConvert.DeserializeObject<List<string>>(v));
		}

        public override int SaveChanges()
		{
			var insertedEntries = this.ChangeTracker.Entries()
													.Where(x => x.State == EntityState.Added)
													.Select(x => x.Entity);
			foreach (var insertedEntry in insertedEntries)
			{
				if (insertedEntry is Auditable auditableEntity)
					auditableEntity.DateCreated = DateTimeOffset.UtcNow;
			}
			var modifiedEntries = this.ChangeTracker.Entries()
													.Where(x => x.State == EntityState.Modified)
													.Select(x => x.Entity);
			foreach (var modifiedEntry in modifiedEntries)
			{
				if (modifiedEntry is Auditable auditableEntity)
					auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
			}

			return base.SaveChanges();
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken=default)
		{
			var insertedEntries = this.ChangeTracker.Entries()
													.Where(x => x.State == EntityState.Added)
													.Select(x => x.Entity);
			foreach (var insertedEntry in insertedEntries)
			{
				if(insertedEntry is Auditable auditableEntity)
					auditableEntity.DateCreated = DateTimeOffset.UtcNow;
			}
			var modifiedEntries = this.ChangeTracker.Entries()
													.Where(x => x.State == EntityState.Modified)
													.Select(x => x.Entity);
			foreach (var modifiedEntry in modifiedEntries)
			{
				if (modifiedEntry is Auditable auditableEntity)
					auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
			}

			return await base.SaveChangesAsync(cancellationToken);
		}
	}
}
