using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zomato.Services.AuthAPI.Models;
using Zomato.Services.AuthAPI.Services.IService;

namespace Zomato.Services.AuthAPI.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        //private readonly IConfiguration _configuration;
        private readonly JwtOptions _jwtOptions;

        public TokenGenerator(IConfiguration configuration, IOptions<JwtOptions> jwtOptions)
        {
            //_configuration = configuration;
            this._jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(ApplicationUser user, IEnumerable<string> roles)
        {
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            var issuer = _jwtOptions.Issuer;
            var audience = _jwtOptions.Audience;

            var tokenHandler = new JwtSecurityTokenHandler();

            var claimlist = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
            };

            claimlist.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimlist),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
