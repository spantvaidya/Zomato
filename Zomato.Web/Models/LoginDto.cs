using System.ComponentModel.DataAnnotations;

namespace Zomato.Web.Models 
{ 
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
