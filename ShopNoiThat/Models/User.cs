using System.ComponentModel.DataAnnotations;

namespace ShopNoiThat.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(50)]
        public string UserName { get; set; } = null!;

        [Required, StringLength(100)]
        public string Password { get; set; } = null!;

        [StringLength(100)]
        public string? FullName { get; set; }

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        [Required, StringLength(20)]
        public string Role { get; set; } = "User";
    }
}
