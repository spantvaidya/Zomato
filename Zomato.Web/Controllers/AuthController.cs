using Microsoft.AspNetCore.Mvc;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;

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
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = _authService.LoginAsync(loginDto);
            if (response == null)
            {
                return Unauthorized(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterationDto registerationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = _authService.RegisterAsync(registerationDto);
            if (response == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
