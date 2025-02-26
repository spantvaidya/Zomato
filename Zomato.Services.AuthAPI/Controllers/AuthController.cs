using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zomato.Services.AuthAPI.Models.Dto;
using Zomato.Services.AuthAPI.Services.IService;
using Zomato.Services.AuthPI.Models.Dto;

namespace Zomato.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        protected ResponseDto _responseDto;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationDto registerationDto)
        {
            var response = await _authService.Register(registerationDto);
            if (!String.IsNullOrEmpty(response))
            {
                _responseDto = new ResponseDto
                {
                    IsSuccess = false,
                    Message = response,
                    StatusCode = StatusCodes.Status400BadRequest
                };
                return BadRequest(_responseDto);
            }

            _responseDto = new ResponseDto
            {
                IsSuccess = true,
                Message = "Registration Successful",
                StatusCode = StatusCodes.Status200OK,
                Result = response
            };

            return Ok(_responseDto);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _authService.Login(loginDto);
            if (user.User == null || String.IsNullOrEmpty(user.Token))
            {
                _responseDto = new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid Credentials",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return Unauthorized(_responseDto);
            }

            _responseDto = new ResponseDto
            {
                IsSuccess = true,
                Message = "Login Successful",
                StatusCode = StatusCodes.Status200OK,
                Result = user
            };
            return Ok(_responseDto);
        }
    }
}
