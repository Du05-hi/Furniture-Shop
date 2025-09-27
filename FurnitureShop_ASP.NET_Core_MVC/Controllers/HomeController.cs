using FurnitureShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        // 🏠 Trang chủ - hiển thị 6 sản phẩm mới nhất
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToListAsync();

            return View(products);
        }

        // 📖 Trang giới thiệu
        public IActionResult About()
        {
            return View();
        }

        // 📞 Trang liên hệ
        public IActionResult Contact()
        {
            return View();
        }

        // ⚠️ Trang lỗi
        public IActionResult Error()
        {
            return View();
        }
    }
}
