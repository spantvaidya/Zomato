using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System.Diagnostics;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;

namespace Zomato.Web.Controllers
{
    public class HomeController(IProductService productService, IShoppingCartService shoppingCartService) : Controller
    {
        private readonly IProductService _productService = productService;
        private readonly IShoppingCartService _cartService = shoppingCartService;

        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetAllProductsAsync();
            if (response != null && response.Result != null)
            {
                var products = JsonConvert.DeserializeObject<List<ProductDto>>(response.Result.ToString());
                return View(products);
            }
            else
                TempData["error"] = response.Message;
            return NotFound();
        }
        [HttpGet("ProductDetails")]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            var response = await _productService.GetProductByIdAsync(productId);
            if (response.IsSuccess)
            {
                var product = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(product);
            }
            else
            {
                TempData["error"] = response.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            CartDto cartDto = new()
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetailsDto = new CartDetailsDto
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };

            List<CartDetailsDto> cartDetailsDtos = new List<CartDetailsDto>();
            cartDetailsDtos.Add(cartDetailsDto);

            cartDto.Cartdetails = cartDetailsDtos;

            ResponseDto? responseDto = await _cartService.UpsertCartAsync(cartDto);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Item added to cart successfully";
                return RedirectToAction(nameof(ProductDetails), productDto);
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(productDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
