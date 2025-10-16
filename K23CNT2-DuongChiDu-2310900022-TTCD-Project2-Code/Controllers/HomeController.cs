using FurnitureShop.Data;
using FurnitureShop.Models;
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

        // 🏠 Trang chủ - hiển thị sản phẩm + bộ lọc tìm kiếm
        public async Task<IActionResult> Index(string? search, decimal? minPrice, decimal? maxPrice, string? sort)
        {
            var products = _db.Products.AsQueryable();

            // 🔍 Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
            }

            // 💰 Lọc theo khoảng giá
            if (minPrice.HasValue)
                products = products.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice.Value);

            // 🔃 Sắp xếp
            products = sort switch
            {
                "price_asc" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                _ => products.OrderByDescending(p => p.Id) // mặc định: mới nhất
            };

            // 🪑 Lấy 4 sản phẩm nổi bật nhất (giá cao nhất)
            ViewBag.FeaturedProducts = await _db.Products
                .OrderByDescending(p => p.Price)
                .Take(4)
                .ToListAsync();

            // Lưu lại các giá trị để hiển thị lại trên form
            ViewBag.Search = search;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Sort = sort;

            // 🪑 Trả về danh sách sản phẩm sau khi lọc
            return View(await products.Take(20).ToListAsync());
        }

        // 📖 Trang giới thiệu
        public IActionResult About() => View();

        // 📞 Trang liên hệ
        public IActionResult Contact() => View();

        // ⚠️ Trang lỗi
        public IActionResult Error() => View();
    }
}
