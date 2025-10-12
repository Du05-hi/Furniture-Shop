using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ✅ BẮT BUỘC để dùng CountAsync, SumAsync, ToListAsync
using FurnitureShop.Data;

namespace FurnitureShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // ... các phương thức ở dưới ...


        // 📊 Dashboard tổng quan
        public async Task<IActionResult> Dashboard()
        {
            // ✅ Thống kê tổng quan
            ViewBag.TotalProducts = await _db.Products.CountAsync();
            ViewBag.TotalUsers = await _db.Users.CountAsync();
            ViewBag.TotalOrders = await _db.Orders.CountAsync();
            ViewBag.TotalRevenue = await _db.OrderDetails.SumAsync(od => od.UnitPrice * od.Quantity);

            // ✅ Doanh thu theo tháng (sửa lỗi aggregate)
            var revenueByMonth = await _db.OrderDetails
                .Where(od => od.Order.OrderDate.Year == DateTime.Now.Year)
                .GroupBy(od => od.Order.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(od => od.UnitPrice * od.Quantity)
                })
                .ToListAsync();

            // ✅ Số đơn hàng theo tháng
            var orderCountByMonth = await _db.Orders
                .Where(o => o.OrderDate.Year == DateTime.Now.Year)
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // ✅ Top 5 sản phẩm bán chạy
            var topProducts = await _db.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => new { od.ProductId, od.Product.Name, od.Product.ImageUrl })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    Name = g.Key.Name,
                    ImageUrl = g.Key.ImageUrl,
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Quantity * od.UnitPrice)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(5)
                .ToListAsync();

            // ✅ Gửi dữ liệu ra View
            ViewBag.RevenueData = System.Text.Json.JsonSerializer.Serialize(revenueByMonth);
            ViewBag.OrderData = System.Text.Json.JsonSerializer.Serialize(orderCountByMonth);
            ViewBag.TopProducts = topProducts;

            return View();
        }
    }
}
