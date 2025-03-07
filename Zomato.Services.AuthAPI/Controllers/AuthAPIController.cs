using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Zomato.MessageBus;
using Zomato.Services.AuthAPI.Models.Dto;
using Zomato.Services.AuthAPI.Services.IService;
using Zomato.Services.AuthPI.Models.Dto;

namespace Zomato.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        protected ResponseDto _responseDto = new();
        private readonly IMessageBus _messageBus = messageBus;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationDto registerationDto)
        {
            var response = await _authService.Register(registerationDto);
            if (!String.IsNullOrEmpty(response))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = response;
                _responseDto.StatusCode = StatusCodes.Status400BadRequest;
                return BadRequest(_responseDto);
            }
            
            _responseDto.StatusCode = StatusCodes.Status200OK;
            _responseDto.Result = response;     

            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _authService.Login(loginDto);
            if (user.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Invalid Credentials";
                _responseDto.StatusCode = StatusCodes.Status400BadRequest;
                return BadRequest(_responseDto);
            }
            _responseDto.StatusCode = StatusCodes.Status200OK;
            _responseDto.Result = user;
            return Ok(_responseDto);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterationDto registerationDto)
        {
            var assignRole = await _authService.AssignRole(registerationDto.Email, registerationDto.Role);
            if (!assignRole)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error Encountered";
                _responseDto.StatusCode = StatusCodes.Status401Unauthorized;
                return Unauthorized(_responseDto);
            }

            _responseDto.StatusCode = StatusCodes.Status200OK;
            _responseDto.Result = assignRole;
            return Ok(_responseDto);
        }

        [HttpPost("EmailRegsiterUser")]
        public async Task<object> EmailRegsiterUser([FromBody] string userEmail)
        {
            try
            {
                //Send Email
                await _messageBus.PublishMessage(userEmail, _configuration.GetSection("TopicAndQueueNames:EmailRegisterQueue").Value);
                _responseDto.Result = true;
                _responseDto.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
                _responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return _responseDto;
        }
    }
}
