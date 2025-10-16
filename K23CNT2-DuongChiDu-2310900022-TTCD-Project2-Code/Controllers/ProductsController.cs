using FurnitureShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db)
        {
            _db = db;
        }

        // 📋 Trang danh sách sản phẩm có lọc + phân trang
        [HttpGet]
        public async Task<IActionResult> Index(string? search, decimal? minPrice, decimal? maxPrice, string? sort, int page = 1)
        {
            int pageSize = 8; // hiển thị 8 sản phẩm mỗi trang

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

            // 🧮 Tính tổng sản phẩm và số trang
            int totalItems = await products.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Đảm bảo trang hợp lệ
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            // 🔢 Lấy dữ liệu theo trang
            var items = await products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            // Truyền dữ liệu sang View
            ViewBag.Search = search;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(items);
        }

        // 📄 Chi tiết sản phẩm
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _db.Products.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }
    }
}
