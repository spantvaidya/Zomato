using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Controllers
{
    public class AuthController(IAuthService authService) : Controller
    {
        private readonly IAuthService _authService = authService;

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
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _authService.LoginAsync(loginDto);
            if (response == null || response.Result == null || response.IsSuccess == false)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(loginDto);
            }

            LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));


            //await _signInManager.SignInAsync(loginResponseDto.User.Email, isPersistent: false);
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
        public IActionResult Logout()
        {
            return View();
        }
    }
}
