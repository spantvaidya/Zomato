namespace Zomato.Services.CoupenAPI.Models
{
    public class Coupon
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public string DiscountAmount { get; set; }
        public string MinAmount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
