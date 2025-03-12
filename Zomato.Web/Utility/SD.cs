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
        public static string ProductAPIBase { get; set; }
        public static string CartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "jwtToken";
        public enum ContentType
        {
            Json,
            MultipartFormData
        }
    }
}
