using Microsoft.AspNetCore.Mvc;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetOrder(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrder/"+ id
            }, true);
        }

        public async Task<ResponseDto?> GetAllOrders(string? userId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrders",
                Data = userId
            }, true);
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.OrderAPIBase + "/api/order/UpdateOrderStatus/"+orderId,
                Data = newStatus
            }, true);
        }

        public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.OrderAPIBase + "/api/order/CreateOrder",
                Data = cartDto
            }, true);
        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.OrderAPIBase + "/api/order/CreateStripeSession",
                Data = stripeRequestDto
            }, true);
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.OrderAPIBase + "/api/order/ValidateStripeSession",
                Data = orderHeaderId
            }, true);
        }
    }
}
