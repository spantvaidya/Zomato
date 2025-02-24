using Microsoft.EntityFrameworkCore;
using Zomato.Services.CoupenAPI.Models;

namespace Zomato.Services.CoupenAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Coupen> Coupens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupen>().HasData(
                new Coupen
                {
                    CoupenCode = "10OFF",
                    CoupenId = 1,
                    DiscountAmount = "10",
                    MinAmount = "50",
                },
                new Coupen
                {
                    CoupenCode = "20OFF",
                    CoupenId = 2,
                    DiscountAmount = "20",
                    MinAmount = "80",
                });
        }
    }
}
