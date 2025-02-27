using Microsoft.AspNetCore.Identity;

namespace Zomato.Web.Utility
{
    public class SD
    {
        public enum ApiType
        {
            GET, POST, PUT, DELETE, TRACE, OPTIONS, HEAD, CONNECT, PATCH
        }
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string RoleAdmin = "Admin";
        public static string RoleCustomer = "Customer";
        public static string TokenCookie = "jwtToken";
    }
}
