using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureShop.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }   // ✅ Kiểu int để trùng AppUser.Id

        [Required]
        public int ProductId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }     // ✅ Liên kết với AppUser

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}
