﻿namespace Zomato.Services.OrderAPI.Models
{
    public class OrderHeaderDto
    {
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double OrderTotal { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime OrderCreated { get; set; }
        public string? OrderStatus { get; set; }
        public IEnumerable<OrderDetails>? OrderDetails { get; set; }
        public string? StripeSessionId { get; set; }
        public string? PaymentIntentId { get; set; }
    }
}
