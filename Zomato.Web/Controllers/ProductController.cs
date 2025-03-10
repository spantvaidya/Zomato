using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;

namespace Zomato.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _ProductService;
        public ProductController(IProductService ProductService)
        {
            _ProductService = ProductService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var responseDto = await _ProductService.GetAllProductsAsync();
                if (responseDto == null || responseDto.Result == null)
                {
                    TempData["error"] = responseDto.Message;
                    return RedirectToAction("Index","Home");
                }
                var Products = JsonConvert.DeserializeObject<List<ProductDto>>(responseDto.Result.ToString());
                return View(Products);
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
        public async Task<IActionResult> Create(ProductDto ProductDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var responseDto = await _ProductService.CreateProductAsync(ProductDto);

                    if (responseDto == null || responseDto.IsSuccess == false)
                    {
                        TempData["error"] = responseDto.Message;
                        return View(ProductDto);
                    }
                    TempData["success"] = "Product Added Succesfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "Invalid Data";
                    return View(ProductDto);
                }
            }

            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }

        public async Task<IActionResult> Update(int productId)
        {
            try
            {
                var responseDto = await _ProductService.GetProductByIdAsync(productId);
                if (responseDto.Result == null)
                {
                    return NotFound();
                }
                var Products = JsonConvert.DeserializeObject<ProductDto>(responseDto.Result.ToString());                
                return View(Products);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductDto ProductDto)
        {
            try
            {
                var responseDto = await _ProductService.UpdateProductAsync(ProductDto);

                if (responseDto.Result == null)
                {
                    TempData["error"] = "Something went wrong";
                    return View(nameof(Update),ProductDto);
                }
                TempData["success"] = "Product Updated Succesfully";
                ProductDto newProdDto = JsonConvert.DeserializeObject<ProductDto>(responseDto.Result.ToString());
                return View(nameof(Update), newProdDto);
            }

            catch(Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }           
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            try
            {
                var responseDto = await _ProductService.GetProductByIdAsync(productId);
                if (responseDto.Result == null)
                {
                    TempData["error"] = "Product Not Found";
                    return RedirectToAction(nameof(Index));
                }

                responseDto = await _ProductService.DeleteProductAsync(productId);

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
