using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopNoiThat.Models;

namespace ShopNoiThat.Data
{
    // Kế thừa IdentityDbContext để dùng User + Role của Identity
    public class ShopNoiThatDbContext : IdentityDbContext<User>
    {
        public ShopNoiThatDbContext(DbContextOptions<ShopNoiThatDbContext> options)
            : base(options)
        {
        }

        // Các DbSet cho bảng khác trong hệ thống
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // Nếu cần, có thể thêm các bảng khác như Category, Cart...
        // public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Có thể thêm cấu hình Fluent API nếu muốn
            // Ví dụ: builder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        }
    }
}
