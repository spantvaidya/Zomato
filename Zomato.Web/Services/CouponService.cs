using Zomato.Web.Models;
using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Services
{
    public class CouponService(IBaseService baseService) : ICouponService
    {
        private readonly IBaseService _baseService = baseService;
        public async Task<ResponseDto?> CreateCouponsAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.CouponAPIBase,
                Data = couponDto
            });
        }

        public async Task<ResponseDto?> DeleteCouponAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto?> GetAllCouponsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto?> GetCouponAsync(string couponCode)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto?> GetCouponByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto?> UpdateCouponsAsync(CouponDto couponDto)
        {
            throw new NotImplementedException();
        }
    }
}
