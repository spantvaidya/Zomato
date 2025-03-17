using Microsoft.AspNetCore.Mvc;
using Zomato.Web.Models;

namespace Zomato.Web.Services.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);
        Task<ResponseDto?> GetAllOrders(string? userId);
        Task<ResponseDto?> GetOrder(int id);
        Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus);
    }
}
