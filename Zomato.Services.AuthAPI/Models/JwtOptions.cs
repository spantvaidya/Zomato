namespace Zomato.Services.AuthAPI.Models
{
    public class JwtOptions
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string ExpirationDate { get; set; }
        public string Audience { get; set; }
    }
}
