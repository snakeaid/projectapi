using Microsoft.EntityFrameworkCore;

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

	}
}

