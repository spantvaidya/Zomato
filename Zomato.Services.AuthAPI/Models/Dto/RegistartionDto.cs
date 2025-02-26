using System.ComponentModel;

namespace Zomato.Services.AuthAPI.Models.Dto
{
    public class RegistartionDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
    }
}
