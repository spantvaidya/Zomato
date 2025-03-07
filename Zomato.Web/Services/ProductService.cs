using Zomato.Web.Models;
using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateProductAsync(ProductDto ProductDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.ProductAPIBase + "/api/Product/AddProduct",
                Data = ProductDto,
                ContentType = SD.ContentType.MultipartFormData
            }, true);
        }

        public async Task<ResponseDto?> DeleteProductAsync(int Id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.ProductAPIBase + "/api/Product/DeleteProduct/" + Id
            }, true);
        }

        public async Task<ResponseDto?> GetAllProductsAsync()
        {
            try
            {
                var result = await _baseService.SendAsync(new RequestDto
                {
                    Apitype = SD.ApiType.GET,
                    Url = SD.ProductAPIBase + "/api/Product/"
                }, true);
                return result;
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ResponseDto?> GetProductAsync(string productName)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/Product/GetProductByName/" + productName
            }, true);
        }

        public async Task<ResponseDto?> GetProductByCategoryAsync(string productCategory)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/Product/GetProductByCategory/" + productCategory
            }, true);
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/Product/GetProductById/" + id
            }, true);
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto ProductDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.PUT,
                Url = SD.ProductAPIBase + "/api/Product/UpdateProduct/",
                Data = ProductDto,
                ContentType = SD.ContentType.MultipartFormData
            }, true);
        }
    }
}
