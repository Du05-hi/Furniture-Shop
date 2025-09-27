using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace FurnitureShop.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string PasswordSalt { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Role { get; set; } = "User";

    // Quan hệ
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    // Hàm hỗ trợ hash mật khẩu
    public static AppUser Create(string username, string password, string role = "User")
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var salt = Convert.ToBase64String(saltBytes);
        var hash = Hash(password, salt);
        return new AppUser { UserName = username, PasswordSalt = salt, PasswordHash = hash, Role = role };
    }

    public static string Hash(string password, string saltBase64)
    {
        var salt = Convert.FromBase64String(saltBase64);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        return Convert.ToBase64String(pbkdf2.GetBytes(32));
    }

    public bool Verify(string password) => PasswordHash == Hash(password, PasswordSalt);
}
