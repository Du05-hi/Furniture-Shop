using FurnitureShop.Data;
using FurnitureShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FurnitureShop.Controllers
{
    [Authorize] // ✅ Chỉ cho phép user đã đăng nhập truy cập
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        // ❤️ Danh sách yêu thích
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            var items = await _context.Wishlist
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return View(items);
        }

        // ➕ Thêm sản phẩm vào yêu thích
        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            bool exists = await _context.Wishlist
                .AnyAsync(w => w.UserId == userId && w.ProductId == id);

            if (!exists)
            {
                var newItem = new Wishlist
                {
                    UserId = userId,
                    ProductId = id,
                    CreatedAt = DateTime.Now
                };

                _context.Wishlist.Add(newItem);
                await _context.SaveChangesAsync();

                TempData["success"] = "Đã thêm sản phẩm vào danh sách yêu thích ❤️";
            }
            else
            {
                TempData["info"] = "Sản phẩm này đã có trong danh sách yêu thích.";
            }

            return RedirectToAction("Index", "Products");
        }

        // 🗑 Xóa 1 sản phẩm yêu thích (chỉ POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            var item = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item == null)
            {
                TempData["error"] = "Không tìm thấy sản phẩm để xóa.";
                return RedirectToAction("Index");
            }

            _context.Wishlist.Remove(item);
            await _context.SaveChangesAsync();

            TempData["success"] = "Đã xóa sản phẩm khỏi danh sách yêu thích.";
            return RedirectToAction("Index");
        }

        // 🧹 Xóa toàn bộ danh sách yêu thích (chỉ POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            var items = _context.Wishlist.Where(w => w.UserId == userId);
            _context.Wishlist.RemoveRange(items);
            await _context.SaveChangesAsync();

            TempData["success"] = "Đã xóa toàn bộ danh sách yêu thích.";
            return RedirectToAction("Index");
        }
    }
}
