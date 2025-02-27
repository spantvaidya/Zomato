using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Zomato.Web.Models
{
    public class RegisterationDto
    {
        public string Name { get; set; }
        [DisplayName("Email Address")]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password should match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [DisplayName("Phone Number")]
        public string? Phone { get; set; }
        public string? Role { get; set; }
    }
}
