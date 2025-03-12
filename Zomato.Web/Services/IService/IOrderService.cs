using Zomato.Web.Models;

namespace Zomato.Web.Services.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
    }
}
