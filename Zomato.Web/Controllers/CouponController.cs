using Microsoft.AspNetCore.Authorization;
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
            try
            {
                var responseDto = await _couponService.GetAllCouponsAsync();
                if (responseDto == null || responseDto.Result == null)
                {
                    TempData["error"] = responseDto.Message;
                    return RedirectToAction("Index","Home");
                }
                var coupons = JsonConvert.DeserializeObject<List<CouponDto>>(responseDto.Result.ToString());
                return View(coupons);
            }
            catch (Exception ex) 
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }           
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponDto couponDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var responseDto = await _couponService.CreateCouponAsync(couponDto);

                    if (responseDto == null)
                    {
                        return NotFound();
                    }
                    TempData["success"] = "Coupon Added Succesfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "Invalid Data";
                    return View(couponDto);
                }
            }

            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }

        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var responseDto = await _couponService.GetCouponByIdAsync(id);
                if (responseDto.Result == null)
                {
                    return NotFound();
                }
                var coupons = JsonConvert.DeserializeObject<CouponDto>(responseDto.Result.ToString());                
                return View(coupons);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(CouponDto couponDto)
        {
            try
            {
                var responseDto = await _couponService.UpdateCouponAsync(couponDto);

                if (responseDto.Result == null)
                {
                    TempData["error"] = "Something went wrong";
                    return View(nameof(Update),couponDto);
                }
                TempData["success"] = "Coupon Updated Succesfully";
                return RedirectToAction(nameof(Index));
            }

            catch(Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }           
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var responseDto = await _couponService.GetCouponByIdAsync(Id);
                if (responseDto.Result == null)
                {
                    TempData["error"] = "Coupon Not Found";
                    return RedirectToAction(nameof(Index));
                }

                responseDto = await _couponService.DeleteCouponAsync(Id);
                if (responseDto.Result == null)
                {
                    TempData["error"] = responseDto.Message;
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }

            catch(Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            
        }
    }
}
