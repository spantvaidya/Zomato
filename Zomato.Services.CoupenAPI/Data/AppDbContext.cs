using Microsoft.EntityFrameworkCore;
using Zomato.Services.CoupenAPI.Models;

namespace Zomato.Services.CoupenAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    CouponCode = "10OFF",
                    CouponId = 1,
                    DiscountAmount = "10",
                    MinAmount = "50",
                },
                new Coupon
                {
                    CouponCode = "20OFF",
                    CouponId = 2,
                    DiscountAmount = "20",
                    MinAmount = "80",
                });
        }
    }
}
