using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FurnitureShop.Data;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Payments
                                .Where(p => p.Status == "Completed")
                                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            ViewData["TotalUsers"] = totalUsers;
            ViewData["TotalProducts"] = totalProducts;
            ViewData["TotalOrders"] = totalOrders;
            ViewData["TotalRevenue"] = totalRevenue;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RevenueChart()
        {
            var data = await _context.Payments
                .Where(p => p.Status == "Completed" && p.PaidAt != null)
                .GroupBy(p => new { p.PaidAt!.Value.Year, p.PaidAt!.Value.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    Total = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            return Json(data);
        }
    }
}
