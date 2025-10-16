using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models;

public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;

    [Required, StringLength(50)]
    public string Method { get; set; } = "COD"; // COD, Banking, Momo...

    [Range(0, 999999999)]
    public decimal Amount { get; set; }

    [Required, StringLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Paid, Failed

    public DateTime? PaidAt { get; set; }
}
