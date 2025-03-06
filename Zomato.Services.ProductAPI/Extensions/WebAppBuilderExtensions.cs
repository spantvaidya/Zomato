using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Zomato.Services.ProductAPI.Extensions
{
    public static class WebAppBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder) 
        {
            //Add Authentication
            var secret = builder.Configuration.GetSection("ApiSettings:Secret").Value;
            var key = Encoding.ASCII.GetBytes(secret);
            var audience = builder.Configuration.GetSection("ApiSettings:Audience").Value;
            var issuer = builder.Configuration.GetSection("ApiSettings:Issuer").Value;

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience
                };
            });

            return builder;
        }
    }
}
