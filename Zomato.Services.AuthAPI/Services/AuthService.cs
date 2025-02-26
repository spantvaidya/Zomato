using Microsoft.AspNetCore.Identity;
using Zomato.Services.AuthAPI.Data;
using Zomato.Services.AuthAPI.Models;
using Zomato.Services.AuthAPI.Models.Dto;
using Zomato.Services.AuthAPI.Services.IService;

namespace Zomato.Services.AuthAPI.Services
{
    public class AuthService(AppDbContext DbContext, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : IAuthService
    {
        private readonly AppDbContext _DbContext = DbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Register(RegisterationDto registerDto)
        {
            ApplicationUser user = new ApplicationUser
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                PhoneNumber = registerDto.Phone,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (result.Succeeded)
                    return "";

                else
                    return result.Errors.FirstOrDefault().Description;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
