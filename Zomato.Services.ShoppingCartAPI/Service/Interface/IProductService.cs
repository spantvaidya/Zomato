using Zomato.Services.CartAPI.Models.Dto;

namespace Zomato.Services.CartAPI.Service.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProducts();
        //Task<ProductDto> GetProductById(int productId);
        //Task<ProductDto> CreateUpdateProduct(ProductDto productDto);
        //Task<bool> DeleteProduct(int productId);
    }
}
