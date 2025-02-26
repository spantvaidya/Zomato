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

        public TokenGenerator(/*IConfiguration configuration,*/ IOptions<JwtOptions> _jwtOptions)
        {
            //_configuration = configuration;
            this._jwtOptions = _jwtOptions.Value;
        }

        public string GenerateToken(ApplicationUser user)
        {
            //var jwtOptions = _configuration.GetSection("ApiSettings:JWTOptions");
            //var key = Encoding.ASCII.GetBytes(jwtOptions["Key"]);
            //var issuer = jwtOptions["Issuer"];
            //var audience = jwtOptions["Audience"];

            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            var issuer = _jwtOptions.Issuer;
            var audience = _jwtOptions.Audience;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim("name", user.Name)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
