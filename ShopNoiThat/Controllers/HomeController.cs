using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNoiThat.Models;

namespace ShopNoiThat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopNoiThatDbContext _context;

        public HomeController(ShopNoiThatDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // lấy 6 sản phẩm mới nhất để hiển thị ở trang chủ
            var products = await _context.Products
                                         .OrderByDescending(p => p.ProductId)
                                         .Take(6)
                                         .ToListAsync();
            return View(products);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
