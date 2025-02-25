using Microsoft.AspNetCore.Http.HttpResults;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Services
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.CouponAPIBase + "/api/coupon/AddCoupon",
                Data = couponDto
            });
        }

        public async Task<ResponseDto?> DeleteCouponAsync(int Id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.CouponAPIBase + "/api/coupon/DeleteCoupon/" + Id
            });
        }

        public async Task<ResponseDto?> GetAllCouponsAsync()
        {
            try
            {
                var result = await _baseService.SendAsync(new RequestDto
                {
                    Apitype = SD.ApiType.GET,
                    Url = SD.CouponAPIBase + "/api/coupon/"
                });
                return result;
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ResponseDto?> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/GetCouponByCode/" + couponCode
            });
        }

        public async Task<ResponseDto?> GetCouponByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/GetCouponById/" + id
            });
        }

        public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.PUT,
                Url = SD.CouponAPIBase + "/api/coupon/UpdateCoupon/",
                Data = couponDto
            });
        }
    }
}
