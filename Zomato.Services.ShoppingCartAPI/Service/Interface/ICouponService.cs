using Zomato.Services.CartAPI.Models;

namespace Zomato.Services.CartAPI.Service.Interface
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}
