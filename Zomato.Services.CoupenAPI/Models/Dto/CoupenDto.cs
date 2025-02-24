namespace Zomato.Services.CoupenAPI.Models
{
    public class CoupenDto
    {
        public int CoupenId { get; set; }
        public string CoupenCode { get; set; }
        public string DiscountAmount { get; set; }
        public string MinAmount { get; set; }
    }
}
