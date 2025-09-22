using System.ComponentModel.DataAnnotations;

namespace ShopNoiThat.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(50)]
        public string UserName { get; set; } = null!;

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [StringLength(100)]
        public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
