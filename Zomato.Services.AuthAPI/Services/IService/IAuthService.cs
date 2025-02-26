using Zomato.Services.AuthAPI.Models.Dto;

namespace Zomato.Services.AuthAPI.Services.IService
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginDto loginDto);
        Task<string> Register(RegisterationDto registerDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}
