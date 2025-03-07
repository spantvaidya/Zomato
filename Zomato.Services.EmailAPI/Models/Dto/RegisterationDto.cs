using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Zomato.Services.EmailAPI.Models.Dto
{
    public class RegisterationDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; }
    }
}
