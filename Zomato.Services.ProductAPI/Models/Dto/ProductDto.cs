using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Zomato.Services.ProductAPI.Models.Dto
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        public double Price { get; set; }

        [Required]
        public string Category { get; set; }

        [Url]
        [ValidateNever]
        public string ImageUrl { get; set; }
    }
}