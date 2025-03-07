using Zomato.Services.EmailAPI.Models.Dto;

namespace Zomato.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task SendAndLogEmailCartAsync(CartDto cartDto);
        Task SendAndLogRegisterUserEmailAsync(RegisterationDto cartDto);
    }
}
