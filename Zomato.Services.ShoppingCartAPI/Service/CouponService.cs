using Newtonsoft.Json;
using System.Net.Http;
using Zomato.Services.CartAPI.Models;
using Zomato.Services.CartAPI.Models.Dto;
using Zomato.Services.CartAPI.Service.Interface;

namespace Zomato.Services.CartAPI.Service
{
    public class CouponService(IHttpClientFactory httpClientFactory) : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            CouponDto Coupon = new CouponDto();
            HttpClient client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/Coupon/GetCouponByCode/" + couponCode);
            var apiContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (apiResponse.IsSuccess)
            {
                Coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(apiResponse.Result));
            }
            return Coupon;
        }       
    }
}
