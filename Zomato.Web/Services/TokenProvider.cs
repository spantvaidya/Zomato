using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }
        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
        }

        public string? GetToken()
        {
            string? token = null;
            bool hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token) ?? false;
            return hasToken ? token : null;
        }

        public void SetToken(string? token)
        {
            if (token == null)
            {
                _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
            }
            else
            {
                CookieOptions options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                _contextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookie, token, options);
            }
        }
    }
}
