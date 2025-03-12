using Zomato.Services.OrderAPI.Models.Dto;

namespace Zomato.Services.OrderAPI.Service.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProducts();
    }
}
