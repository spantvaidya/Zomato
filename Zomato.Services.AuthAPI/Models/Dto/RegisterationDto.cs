using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Zomato.Services.AuthAPI.Models.Dto
{
    public class RegisterationDto
    {
        public string Name { get; set; }
        [DisplayName("Email Address")]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password should match")]
        public string ConfirmPassword { get; set; }
        [DisplayName("Phone Number")]
        public string? Phone { get; set; }
        public string? Role { get; set; }
    }
}
