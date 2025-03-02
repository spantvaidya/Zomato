using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;

namespace Zomato.Web.Controllers
{
    public class HomeController(IProductService productService) : Controller
    {
        private readonly IProductService _productService = productService;

        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetAllProductsAsync();
            var products = JsonConvert.DeserializeObject<List<ProductDto>>(response.Result.ToString());
            return View(products);
        }

        public async Task<IActionResult> ProductDetails(int productId)
        {
            var response = await _productService.GetProductByIdAsync(productId);
            if(response.IsSuccess)
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
