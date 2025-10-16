using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Data;
using FurnitureShop.Models;
using System.Security.Claims;

namespace FurnitureShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _db;

        public CartController(AppDbContext db)
        {
            _db = db;
        }

        // 🛒 Xem giỏ hàng
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cart = await _db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(cart);
        }

        // ➕ Thêm vào giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var item = await _db.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                item.Quantity += quantity;
                _db.Update(item);
            }
            else
            {
                _db.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // ❌ Xóa khỏi giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var item = await _db.CartItems.FindAsync(id);
            if (item != null)
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // 💸 Áp dụng mã giảm giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                TempData["Error"] = "Vui lòng nhập mã giảm giá.";
                return RedirectToAction("Index");
            }

            // Tìm mã trong DB (chỉ nhận mã còn hạn)
            var coupon = await _db.Coupons
                .FirstOrDefaultAsync(c => c.Code == couponCode &&
                                     (c.ExpiryDate == null || c.ExpiryDate >= DateTime.Now));

            if (coupon == null)
            {
                TempData["Error"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Index");
            }

            // Lưu thông tin vào Session để hiển thị và tính toán
            HttpContext.Session.SetString("AppliedCoupon", coupon.Code);
            HttpContext.Session.SetString("DiscountPercent", coupon.DiscountPercent.ToString());

            TempData["Success"] = $"Áp dụng mã '{coupon.Code}' thành công! Giảm {coupon.DiscountPercent}%.";

            return RedirectToAction("Index");
        }
    }
}
