using Microsoft.EntityFrameworkCore;
using Zomato.Services.ProductAPI.Models;

namespace Zomato.Services.ProductAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "Product1", Price = 10.0, Description = "Description1", Category = "Category1", ImageUrl = "http://example.com/image1" },
                new Product { ProductId = 2, Name = "Product2", Price = 20.0, Description = "Description2", Category = "Category2", ImageUrl = "http://example.com/image2" }
            );
        }
    }
}
