using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Zomato.MessageBus;
using Zomato.Services.CartAPI;
using Zomato.Services.CartAPI.Data;
using Zomato.Services.CartAPI.Extensions;
using Zomato.Services.CartAPI.Service;
using Zomato.Services.CartAPI.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme as: `Bearer Generated-Jwt-Toke`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            new string[] {}
        }
    });
});

//Add HttpClient for calling Product, Coupon Microservices
builder.Services.AddHttpClient("Product", x => x.BaseAddress =
new Uri(builder.Configuration.GetSection("ServiceUrls:ProductAPI").Value));
builder.Services.AddHttpClient("Coupon", x => x.BaseAddress =
new Uri(builder.Configuration.GetSection("ServiceUrls:CouponAPI").Value));

builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddDbContext<AppDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.AddAppAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment())
    app.UseSwaggerUI();
else
{
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthAPI");
        x.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//Add SeedData and Pending Migration at the start of the App
ApplyMigration();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitilizer = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (dbInitilizer.Database.GetPendingMigrations().Count() > 0)
        {
            dbInitilizer.Database.Migrate();
        };
    }
}


app.Run();