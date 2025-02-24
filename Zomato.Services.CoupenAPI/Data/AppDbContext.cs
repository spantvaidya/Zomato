using Microsoft.EntityFrameworkCore;
using Zomato.Services.CoupenAPI.Models;

namespace Zomato.Services.CoupenAPI.Data
{
    public class AppDbContext : DbContext
    {
        DbSet<Coupen> Coupens { get; set; }
    }
}
