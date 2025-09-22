using Microsoft.AspNetCore.Identity;

namespace ShopNoiThat.Models
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
