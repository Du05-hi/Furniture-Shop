using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; } = default!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required, StringLength(20)]
    public string Status { get; set; } = "Pending";

    // Quan hệ
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public Payment? Payment { get; set; }
}
