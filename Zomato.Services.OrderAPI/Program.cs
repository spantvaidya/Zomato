using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Zomato.MessageBus;
using Zomato.Services.OrderAPI;
using Zomato.Services.OrderAPI.Data;
using Zomato.Services.OrderAPI.Extensions;
using Zomato.Services.OrderAPI.Service;
using Zomato.Services.OrderAPI.Service.Interface;
using Zomato.Services.OrderAPI.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendAPIAuthenticationHttpClientHandler>();

//Add HttpClient for calling Product, Coupon Microservices
builder.Services.AddHttpClient("Product", x => x.BaseAddress =
new Uri(builder.Configuration.GetSection("ServiceUrls:ProductAPI").Value))
    .AddHttpMessageHandler<BackendAPIAuthenticationHttpClientHandler>();

builder.AddAppAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IMessageBus, MessageBus>();

builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddDbContext<AppDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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

Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Value;

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
