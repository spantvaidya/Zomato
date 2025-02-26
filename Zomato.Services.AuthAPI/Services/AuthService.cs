using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zomato.Services.AuthAPI.Data;
using Zomato.Services.AuthAPI.Models;
using Zomato.Services.AuthAPI.Models.Dto;
using Zomato.Services.AuthAPI.Services.IService;

namespace Zomato.Services.AuthAPI.Services
{
    public class AuthService(AppDbContext DbContext, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, ITokenGenerator tokenGenerator) : IAuthService
    {
        private readonly AppDbContext _DbContext = DbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly ITokenGenerator _tokenGenerator = tokenGenerator;

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = await _DbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null) 
            {
                if(!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName.ToUpper()));
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var user = await _DbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == loginDto.Username);

            bool isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (user == null || !isValid)
            {
                return new LoginResponseDto
                {
                    Token = "",
                    User = null
                };
            }

            //generate JWT token
            string jwtToken = _tokenGenerator.GenerateToken(user);

            UserDto userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto
            {
                User = userDto,
                Token = jwtToken
            };

            return loginResponseDto;
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
