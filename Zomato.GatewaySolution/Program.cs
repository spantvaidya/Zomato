using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOcelot();

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

var app = builder.Build();
app.UseOcelot().Wait();

app.MapGet("/", () => "Hello World!");

app.Run();
