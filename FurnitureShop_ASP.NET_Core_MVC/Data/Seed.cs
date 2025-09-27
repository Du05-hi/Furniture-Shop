using FurnitureShop.Models;

namespace FurnitureShop.Data;

public static class Seed
{
    public static void Init(AppDbContext db)
    {
        // Seed Users
        if (!db.Users.Any())
        {
            var admin = AppUser.Create("admin", "admin123", "Admin");
            var user = AppUser.Create("user", "user123", "User");

            db.Users.AddRange(admin, user);
            db.SaveChanges();
        }

        // Seed Products
        if (!db.Products.Any())
        {
            db.Products.AddRange(
                new Product { Name = "Sofa da cao cấp", Price = 12000000, Description = "Sofa da thật êm ái, phong cách hiện đại", ImageUrl = "/images/sofa.jpg" },
                new Product { Name = "Bàn gỗ tự nhiên", Price = 3500000, Description = "Bàn ăn gỗ sồi nguyên khối", ImageUrl = "/images/ban.jpg" },
                new Product { Name = "Tủ quần áo 3 cánh", Price = 7500000, Description = "Tủ gỗ MDF phủ melamine chống ẩm", ImageUrl = "/images/tu.jpg" }
            );
            db.SaveChanges();
        }
    }
}
