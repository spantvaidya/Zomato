using Zomato.Web.Models;

namespace Zomato.Web.Services.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> RegisterAsync(RegisterationDto registerationDto);
        Task<ResponseDto?> LoginAsync(LoginDto loginDto);
        Task<ResponseDto?> AssignRoleAsync(RegisterationDto registerationDto);
    }
}
