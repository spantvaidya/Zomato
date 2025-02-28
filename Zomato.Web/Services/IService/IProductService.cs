using Zomato.Web.Models;

namespace Zomato.Web.Services.IService
{
    public interface IProductService
    {
        Task<ResponseDto?> GetProductAsync(string productName);
        Task<ResponseDto?> GetProductByCategoryAsync(string productCategory);
        Task<ResponseDto?> GetAllProductsAsync();
        Task<ResponseDto?> GetProductByIdAsync(int productId);
        Task<ResponseDto?> CreateProductAsync(ProductDto productDto);
        Task<ResponseDto?> UpdateProductAsync(ProductDto productDto);
        Task<ResponseDto?> DeleteProductAsync(int productId);
    }
}
