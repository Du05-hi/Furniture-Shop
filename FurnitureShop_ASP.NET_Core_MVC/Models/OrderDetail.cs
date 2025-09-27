using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models;

public class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    [Range(1, 999)]
    public int Quantity { get; set; }

    [Range(0, 999999999)]
    public decimal UnitPrice { get; set; }
}
