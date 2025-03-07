using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Zomato.Services.ProductAPI.Models.Dto
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0.01, 10000.00)]
        public double Price { get; set; }

        public string Category { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
        public IFormFile? Image { get; set; }
    }
}