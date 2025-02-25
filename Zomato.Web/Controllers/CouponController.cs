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

        [HttpPost]
        public async Task<IActionResult> Create(CouponDto couponDto)
        {
            var responseDto = await _couponService.CreateCouponAsync(couponDto);

            if (responseDto == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Update(int id)
        {
            var responseDto = await _couponService.GetCouponByIdAsync(id);
            if (responseDto.Result == null)
            {                
                return NotFound();
            }
            var coupons = JsonConvert.DeserializeObject<CouponDto>(responseDto.Result.ToString());
            return View(coupons);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CouponDto couponDto)
        {
            var responseDto = await _couponService.UpdateCouponAsync(couponDto);

            if (responseDto == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            var responseDto = await _couponService.GetCouponByIdAsync(Id);
            if (responseDto.Result == null)
            {
                return NotFound();
            }

            responseDto = await _couponService.DeleteCouponAsync(Id);

            if (responseDto == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
