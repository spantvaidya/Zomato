﻿using Microsoft.AspNetCore.Identity;

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

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadyForPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Cancelled = "Cancelled";
        public const string Status_Refunded = "Refunded";

        public enum ContentType
        {
            Json,
            MultipartFormData
        }
    }
}
