using Zomato.Web.Models;
using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> AssignRoleAsync(RegisterationDto registerationDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.AuthAPIBase + "/api/auth/AssignRole",
                Data = registerationDto
            },true);
        }

        public async Task<ResponseDto?> LoginAsync(LoginDto loginDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.AuthAPIBase + "/api/auth/login",
                Data = loginDto
            }, true);
        }

        public async Task<ResponseDto?> RegisterAsync(RegisterationDto registerationDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                Apitype = SD.ApiType.POST,
                Url = SD.AuthAPIBase + "/api/auth/register",
                Data = registerationDto
            }, true);
        }
    }
}
