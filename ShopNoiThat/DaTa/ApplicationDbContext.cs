using Microsoft.AspNetCore.Identity;
using System;

namespace MyWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
