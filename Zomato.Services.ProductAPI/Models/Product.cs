using System.ComponentModel.DataAnnotations;

namespace Zomato.Services.ProductAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(0.01, 10000.00)]
        public double Price { get; set; }

        [Required]
        public string Category { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
    }
}
