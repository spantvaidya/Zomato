namespace Zomato.Services.EmailAPI.Utility
{
    using System;
    using System.Text;
    using Microsoft.Extensions.Configuration;

    public static class EmailTemplate
    {
        public static string GetEmailCartBody(string userName)
        {
            string baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
            string orderUrl = $"{baseUrl}/Home";

            // Build email body
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append("<style>");
            sb.Append("body { font-family: Arial, sans-serif; background-color: #f8f9fa; margin: 0; padding: 0; }");
            sb.Append(".container { max-width: 600px; margin: auto; background: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }");
            sb.Append(".header { background: #ff5722; color: #fff; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; border-radius: 8px 8px 0 0; }");
            sb.Append(".content { padding: 20px; text-align: center; color: #333; font-size: 16px; }");
            sb.Append(".button { background: #ff5722; color: #fff; text-decoration: none; padding: 10px 20px; border-radius: 5px; display: inline-block; font-size: 18px; margin-top: 15px; }");
            sb.Append(".footer { text-align: center; padding: 15px; font-size: 14px; color: #777; }");
            sb.Append("</style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<div class='container'>");
            sb.Append("<div class='header'>Welcome to ZomatoWeb!</div>");
            sb.Append("<div class='content'>");
            sb.Append($"<p>Hey {userName},</p>");
            sb.Append("<p>Congratulations! You are now part of the <b>ZomatoWeb</b> family, where delicious meals are just a click away.</p>");
            sb.Append("<p>From sizzling street food to fine dining, we've got it all covered. Start exploring now!</p>");
            sb.Append($"<a href='{orderUrl}' class='button'>Start Ordering</a>");
            sb.Append("<p>Bon Appétit!</p>");
            sb.Append("</div>");
            sb.Append("<div class='footer'>");
            sb.Append("<p>&copy; 2025 ZomatoWeb. All rights reserved.</p>");
            sb.Append("<p>Need help? <a href='mailto:sameer.pantvaidya@gmail.com'>Contact Support</a></p>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</body>");
            sb.Append("</html>");

            return sb.ToString();
        }
    }

}
