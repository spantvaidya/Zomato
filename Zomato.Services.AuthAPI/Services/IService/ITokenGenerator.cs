using Zomato.Services.AuthAPI.Models;

namespace Zomato.Services.AuthAPI.Services.IService
{
    public interface ITokenGenerator
    {
        string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    }
}
