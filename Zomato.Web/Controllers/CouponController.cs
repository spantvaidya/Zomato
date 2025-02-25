using Microsoft.AspNetCore.Mvc;
using Zomato.Web.Services.IService;

namespace Zomato.Web.Controllers
{
    public class CouponController(ICouponService couponService) : Controller
    {
        private readonly ICouponService _couponService = couponService;

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Create()
        {
            return View();
        }
    }
}
