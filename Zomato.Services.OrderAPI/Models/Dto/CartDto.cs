﻿namespace Zomato.Services.OrderAPI.Models.Dto
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; }
        public IEnumerable<CartDetailsDto>? Cartdetails { get; set; }
    }
}
