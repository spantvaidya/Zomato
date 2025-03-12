using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System.Security.Claims;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;

namespace Zomato.Web.Controllers
{
    public class ShoppingCartController(IShoppingCartService cartService, IOrderService orderService) : Controller
    {
        private readonly IShoppingCartService _cartService = cartService;
        private readonly IOrderService _orderService = orderService;

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoByLoggedInUserAsync());
        }

        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            return View(await LoadCartDtoByLoggedInUserAsync());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckOut(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoByLoggedInUserAsync();
            cart.CartHeader.Name = cartDto.CartHeader.Name;
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;

            var response = await _orderService.CreateOrder(cart);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(response.Result.ToString());
                //get stripe payment details

            }
            return View();
        }

        private async Task<CartDto> LoadCartDtoByLoggedInUserAsync()
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

        [HttpPost("EmailCart")]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoByLoggedInUserAsync();
            cart.CartHeader.Email = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

            var response = await _cartService.EmailCart(cart);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Email will be sent Shortly";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
    }
}
