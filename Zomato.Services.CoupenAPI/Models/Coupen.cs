namespace Zomato.Services.CoupenAPI.Models
{
    public class Coupen
    {
        public int CoupenId { get; set; }
        public string CoupenCode { get; set; }
        public string DiscountAmount { get; set; }
        public string MinAmount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
