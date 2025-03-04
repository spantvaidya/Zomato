using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System.Security.Claims;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;

namespace Zomato.Web.Controllers
{
    public class ShoppingCartController(IShoppingCartService cartService) : Controller
    {
        private readonly IShoppingCartService _cartService = cartService;

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDto());
        }

        private async Task<CartDto> LoadCartDto()
        {
            var userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CartDto>(response.Result.ToString());
            }
            return new CartDto();
        }

        [HttpPost("ClearCart")]
        public async Task<IActionResult> ClearCartAsync(int cartDetailsId)
        {
            var userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _cartService.ClearCartAsync(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                var cartDto = JsonConvert.DeserializeObject<CartDto>(response.Result.ToString());
                return View("CartIndex", cartDto);
            }
            return null;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<IActionResult> ApplyCouponAsync(CartDto cartDto)
        {
            var response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Coupon applied successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost("RemoveCoupon")]
        public async Task<IActionResult> RemoveCouponAsync(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = string.Empty;
            var response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Coupon removed successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
    }
}
