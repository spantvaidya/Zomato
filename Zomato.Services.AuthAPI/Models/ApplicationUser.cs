﻿using Microsoft.AspNetCore.Identity;

namespace Zomato.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
