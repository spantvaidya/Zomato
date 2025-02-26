using System.ComponentModel.DataAnnotations;

namespace Zomato.Services.CoupenAPI.Models
{
    public class CouponDto
    {
        public int CouponId { get; set; }

        [Required(ErrorMessage = "Coupon Code is required")]
        [StringLength(50, ErrorMessage = "Coupon Code cannot be longer than 50 characters")]
        public string CouponCode { get; set; }

        [Required(ErrorMessage = "Discount Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount Amount must be greater than zero")]
        public decimal DiscountAmount { get; set; }

        [Required(ErrorMessage = "Minimum Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Minimum Amount must be greater than zero")]
        public decimal MinAmount { get; set; }
    }
}
