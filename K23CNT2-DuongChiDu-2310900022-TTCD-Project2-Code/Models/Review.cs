using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models;

public class Review
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; } = default!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
