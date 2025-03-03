using Newtonsoft.Json;
using Zomato.Services.CartAPI.Models.Dto;
using Zomato.Services.CartAPI.Service.Interface;

namespace Zomato.Services.CartAPI.Service
{
    public class ProductService(IHttpClientFactory httpClientFactory) : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            List<ProductDto> products = new List<ProductDto>();
            HttpClient client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (apiResponse.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(apiResponse.Result));
            }
            return products;
        }
    }
}
