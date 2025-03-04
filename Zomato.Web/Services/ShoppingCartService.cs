using Zomato.Web.Models;
using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Services
{
    public class ShoppingCartService(IBaseService baseService) : IShoppingCartService
    {
        private readonly IBaseService _baseService = baseService;

        public async Task<ResponseDto?> ApplyCouponAsync(CartDto CartDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Data = CartDto,
                Url = SD.CartAPIBase + "/api/cart/ApplyCoupon/"
            }, true);
        }

        public async Task<ResponseDto?> ClearCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Data = cartDetailsId,
                Url = SD.CartAPIBase + "/api/cart/ClearCart/"
            }, true);
        }

        public async Task<ResponseDto?> EmailCart(CartDto CartDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Data = CartDto,
                Url = SD.CartAPIBase + "/api/cart/EmailCartRequest/"
            }, true);
        }

        public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.GET,
                Url = SD.CartAPIBase + "/api/cart/GetCart/" + userId
            }, true);
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartDto CartDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Data = CartDto,
                Url = SD.CartAPIBase + "/api/cart/CartUpsert/"
            }, true);
        }
    }
}
