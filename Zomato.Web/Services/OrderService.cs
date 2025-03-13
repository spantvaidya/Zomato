using Microsoft.AspNetCore.Http.HttpResults;
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
