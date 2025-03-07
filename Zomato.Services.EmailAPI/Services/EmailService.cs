using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;
using Zomato.Services.EmailAPI.Data;
using Zomato.Services.EmailAPI.Models;
using Zomato.Services.EmailAPI.Models.Dto;
using Zomato.Services.EmailAPI.Utility;

namespace Zomato.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            this._dbOptions = dbOptions;
        }

        public async Task SendAndLogEmailCartAsync(CartDto cartDto)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("<h1>Cart Received</h1>");
            stringBuilder.AppendLine($"<p>Dear {cartDto.CartHeader.FirstName ?? "Guest"} {cartDto.CartHeader.LastName},</p>");
            stringBuilder.AppendLine("<p>Thank you for Cart request. Here are the Cart details:</p>");
            stringBuilder.AppendLine("<ul>");

            foreach (var item in cartDto.Cartdetails)
            {
                stringBuilder.AppendLine($"<li>{item.ProductDto.Name} - {item.Count} x {item.ProductDto.Price:C}</li>");
            }

            stringBuilder.AppendLine("</ul>");
            stringBuilder.AppendLine($"<p>Total: {cartDto.CartHeader.CartTotal:C}</p>");
            stringBuilder.AppendLine("<p>We will notify you once your order is shipped.</p>");
            stringBuilder.AppendLine("<p>Best regards,</p>");
            stringBuilder.AppendLine("<p>Zomato Team</p>");

            var emailBody = stringBuilder.ToString();


            //log email
            if (await SendEmail(cartDto.CartHeader.Email, emailBody, "Cart Received"))
                await LogEmail(emailBody, cartDto.CartHeader.Email??"");
        }

        public async Task SendAndLogRegisterUserEmailAsync(RegisterationDto registerationDto)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(RegisterEmailtemplate.GetRegisterUserEmailBody(registerationDto.Name));

            var emailBody = stringBuilder.ToString();

            //log email
            if (await SendEmail(registerationDto.Email, emailBody, "Welcome To Zomato"))
                await LogEmail(emailBody, registerationDto.Email ?? "");
        }

        private async Task<bool> SendEmail(string toEmail, string emailBody, string subject)
        {
            try
            {
                using (var client = new SmtpClient(SD.SMTPMailClientHost)
                {
                    Port = SD.SMTPMailClientPort,
                    Credentials = new NetworkCredential(SD.EmailFrom, SD.EmailFromPassword),
                    EnableSsl = true,
                })
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("sameer.pantvaidya@gmail.com"),
                        Subject = subject,
                        Body = emailBody,
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add(toEmail);                   

                    await client.SendMailAsync(mailMessage);
                }
                return true;
            }
            catch (Exception ex)
            {
                //TempData["error"] = ex.Message;
                return false;
            }
        }

        private async Task<bool> LogEmail(string message, string Email)
        {
            try
            {
                using (var dbContext = new AppDbContext(_dbOptions))
                {
                    var emailLogger = new EmailLogger
                    {
                        Email = Email,
                        Message = message,
                        EmailSent = DateTime.Now
                    };
                    dbContext.EmailLoggers.Add(emailLogger);
                    await dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
    }
}
