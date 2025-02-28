using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Controllers
{
    public class AuthController(IAuthService authService, ITokenProvider tokenProvider) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly ITokenProvider _tokenProvider = tokenProvider;

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegisterationDto registerationDto = new RegisterationDto
            {
                Role = "Customer"
            };
            return View(registerationDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _authService.LoginAsync(loginDto);
            if (response == null || response.Result == null || response.IsSuccess == false)
            {
                TempData["error"]= "Invalid Credetials";
                return View(loginDto);
            }

            LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));

            // Signin User and Set token for logged in user
            await SignInUser(loginResponseDto);
            _tokenProvider.SetToken(loginResponseDto.Token);

            TempData["success"] = "Login Successful";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterationDto registerationDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid Data";
                return BadRequest(ModelState);
            }
            ResponseDto responseDto = await _authService.RegisterAsync(registerationDto);
            ResponseDto responseAssignRole;
            if (responseDto == null || responseDto.IsSuccess == false)
            {
                TempData["error"] = "Something went wrong";
                return BadRequest(responseDto);
            }

            if (String.IsNullOrEmpty(registerationDto.Role))
            {
                registerationDto.Role = SD.RoleCustomer;
            }
            responseAssignRole = await _authService.AssignRoleAsync(registerationDto);

            if (responseAssignRole != null || responseAssignRole.IsSuccess == true)
            {
                TempData["success"] = "Registration Successful";
                return RedirectToAction(nameof(Login), "Auth");
            }

            return View(registerationDto);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        public async Task SignInUser(LoginResponseDto loginResponseDto)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(loginResponseDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, 
                 jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                 jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                 jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, 
                jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, 
                jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
        }
    }
}
