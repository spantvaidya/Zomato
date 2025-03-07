using Microsoft.EntityFrameworkCore;
using Zomato.Services.EmailAPI;
using Zomato.Services.EmailAPI.Data;
using Zomato.Services.EmailAPI.Extensions;
using Zomato.Services.EmailAPI.Messaging;
using Zomato.Services.EmailAPI.Services;
using Zomato.Services.EmailAPI.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddDbContext<AppDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailService(optionsBuilder.Options));

//Set SMTP settings
var emailSettings = builder.Configuration.GetSection("EmailSettings");

SD.SMTPMailClientHost = builder.Configuration.GetSection("EmailSettings:Host").Value;
SD.SMTPMailClientPort = Convert.ToInt32(builder.Configuration.GetSection("EmailSettings:Port").Value);
SD.EmailFrom = builder.Configuration.GetSection("EmailSettings:MailFromAdmin").Value;
SD.EmailFromPassword = builder.Configuration.GetSection("EmailSettings:MailFromAdminPassword").Value;

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment())
    app.UseSwaggerUI();
else
{
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "Email API");
        x.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Add Pending Migration at the start of the App
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
app.UseAzureServiceBusConsumer();
app.Run();
