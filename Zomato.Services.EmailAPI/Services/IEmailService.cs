using Zomato.Services.EmailAPI.Message;
using Zomato.Services.EmailAPI.Models.Dto;

namespace Zomato.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task SendAndLogEmailCartAsync(CartDto cartDto);
        Task SendAndLogRegisterUserEmailAsync(string email);
        Task SendAndLogOrderCreatedEmailAsync(EmailMessage emailMessage);
    }
}
