using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;

namespace Zomato.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            var responseDto = await _couponService.GetAllCouponsAsync();
            if (responseDto == null)
            {
                return NotFound();
            }
            var coupons = JsonConvert.DeserializeObject<List<CouponDto>>(responseDto.Result.ToString());
            return View(coupons);
        }
        
        public IActionResult Create()
        {
            return View();
        }
    }
}
