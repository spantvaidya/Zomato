using Zomato.Web.Models;

namespace Zomato.Web.Services.IService
{
    public interface IShoppingCartService
    {
        Task<ResponseDto?> GetCartByUserIdAsync(string userId);
        Task<ResponseDto?> UpsertCartAsync(CartDto CartDto);
        Task<ResponseDto?> ClearCartAsync(int cartDetailsId);
        Task<ResponseDto?> ApplyCouponAsync(CartDto CartDto);
        Task<ResponseDto?> EmailCart(CartDto CartDto);
    }
}
