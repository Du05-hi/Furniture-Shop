using FurnitureShop.Data;
using FurnitureShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Controllers
{
    // ✅ Bắt buộc đăng nhập cho tất cả action, trừ những nơi được AllowAnonymous
    [Authorize]
    public class CouponsController : Controller
    {
        private readonly AppDbContext _db;

        public CouponsController(AppDbContext db)
        {
            _db = db;
        }

        // 📋 Hiển thị danh sách mã giảm giá (chỉ Admin được xem)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var coupons = await _db.Coupons
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(coupons);
        }

        // ➕ Hiển thị form thêm mới (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 💾 Xử lý thêm mã giảm giá (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 🔍 Kiểm tra trùng mã
            bool exists = await _db.Coupons.AnyAsync(c => c.Code == model.Code);
            if (exists)
            {
                ModelState.AddModelError("Code", "❌ Mã giảm giá này đã tồn tại!");
                return View(model);
            }

            _db.Coupons.Add(model);
            await _db.SaveChangesAsync();

            TempData["success"] = "✅ Thêm mã giảm giá thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ✏️ Hiển thị form chỉnh sửa (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _db.Coupons.FindAsync(id);
            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        // 💾 Cập nhật mã giảm giá (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var existing = await _db.Coupons.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existing == null)
                return NotFound();

            _db.Coupons.Update(model);
            await _db.SaveChangesAsync();

            TempData["success"] = "✏️ Cập nhật mã giảm giá thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 🗑️ Xóa mã giảm giá (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _db.Coupons.FindAsync(id);
            if (coupon == null)
                return NotFound();

            _db.Coupons.Remove(coupon);
            await _db.SaveChangesAsync();

            TempData["success"] = "🗑️ Đã xóa mã giảm giá!";
            return RedirectToAction(nameof(Index));
        }

        // 🎟️ Áp dụng mã giảm giá (User hoặc khách)
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(string code)
        {
            // ⚠️ Nếu người dùng chưa nhập
            if (string.IsNullOrWhiteSpace(code))
            {
                TempData["error"] = "⚠️ Vui lòng nhập mã giảm giá.";
                return RedirectToAction("Index", "Cart");
            }

            // 🔎 Tìm mã hợp lệ trong CSDL
            var coupon = await _db.Coupons.FirstOrDefaultAsync(c =>
                c.Code == code &&
                c.IsActive &&
                (!c.ExpiryDate.HasValue || c.ExpiryDate.Value > DateTime.Now));

            if (coupon == null)
            {
                TempData["error"] = "❌ Mã giảm giá không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Index", "Cart");
            }

            // ✅ Mã hợp lệ → lưu thông tin sang giỏ hàng
            TempData["success"] = $"🎉 Mã '{coupon.Code}' được áp dụng! Giảm {coupon.DiscountPercent}% cho đơn hàng của bạn.";
            TempData["AppliedCoupon"] = coupon.Code;
            TempData["DiscountPercent"] = coupon.DiscountPercent;

            // 👉 Quay lại trang giỏ hàng hiển thị tổng tiền sau giảm
            return RedirectToAction("Index", "Cart");
        }
    }
}
