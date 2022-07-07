using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.DataAccess.Primitives.Abstractions;

namespace ProjectAPI.DataAccess
{
	/// <summary>
	/// Database context class which represents the product catalog and implements <see cref="DbContext"/>.
	/// </summary>
	public class CatalogContext : DbContext
	{
		/// <summary>
		/// Gets and sets the instance of <see cref="DbSet{Category}"/> for <see cref="Category"/>, used for database interactions.
		/// </summary>
		public DbSet<Category> Categories { get; set; }

		/// <summary>
		/// Gets and sets the instance of <see cref="DbSet{Product}"/> for <see cref="Product"/>, used for database interactions.
		/// </summary>
		public DbSet<Product> Products { get; set; }

		/// <summary>
        /// Constructs a new instance of <see cref="CatalogContext"/> class using the specified options.
        /// </summary>
        /// <param name="options">An instance of <see cref="DbContextOptions{TContext}"/> class for <see cref="CatalogContext"/>.</param>
		public CatalogContext(DbContextOptions<CatalogContext> options)
			: base(options)
		{
						//Database.EnsureDeleted();
			Database.EnsureCreated();
		}

		/// <summary>
		/// Builds the model and its mappings in memory when the context is first created.
		/// </summary>
        /// <param name="modelBuilder">An instance of <see cref="ModelBuilder"/> class.</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				if (typeof(IAuditable).IsAssignableFrom(entityType.ClrType))
				{
					var parameter = Expression.Parameter(entityType.ClrType, "p");
					var deletedCheck = Expression.Lambda(Expression.Equal(Expression.Property(parameter, "DateDeleted"),
														 Expression.Constant(null)), parameter);
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

			modelBuilder.Entity<Category>().HasData(new Category { Id = 1, Name = "Uncategorized", Description = "Products without a specific category are stored here", DateCreated=DateTimeOffset.UtcNow });
		}

		/// <summary>
		/// Saves all changes made in the context of the database. 
		/// </summary>
		public override int SaveChanges()
		{
			var insertedEntries = this.ChangeTracker.Entries()
													.Where(x => x.State == EntityState.Added)
													.Select(x => x.Entity);
			foreach (var insertedEntry in insertedEntries)
			{
				if (insertedEntry is IAuditable auditableEntity)
					auditableEntity.DateCreated = DateTimeOffset.UtcNow;
			}
			var modifiedEntries = this.ChangeTracker.Entries()
													.Where(x => x.State == EntityState.Modified)
													.Select(x => x.Entity);
			foreach (var modifiedEntry in modifiedEntries)
			{
				if (modifiedEntry is IAuditable auditableEntity)
					auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
			}

			return base.SaveChanges();
		}

		/// <summary>
		/// Asynchronously saves all changes made in the context of the database. 
		/// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="int"/></returns>
		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var insertedEntries = this.ChangeTracker.Entries()
													.Where(x => x.State == EntityState.Added)
													.Select(x => x.Entity);
			foreach (var insertedEntry in insertedEntries)
			{
				if (insertedEntry is IAuditable auditableEntity)
					auditableEntity.DateCreated = DateTimeOffset.UtcNow;
			}
			var modifiedEntries = this.ChangeTracker.Entries()
													.Where(x => x.State == EntityState.Modified)
													.Select(x => x.Entity);
			foreach (var modifiedEntry in modifiedEntries)
			{
				if (modifiedEntry is IAuditable auditableEntity)
					auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
			}

			return await base.SaveChangesAsync(cancellationToken);
		}
	}
}
