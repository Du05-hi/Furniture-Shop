using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Data;
using FurnitureShop.Models;
using System.Security.Claims;

namespace FurnitureShop.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _db;

        public OrdersController(AppDbContext db)
        {
            _db = db;
        }

        // 🛒 User: xem đơn hàng của chính mình
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(AdminIndex));

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orders = await _db.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // 👨‍💻 Admin: xem tất cả đơn hàng
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var orders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // 📊 Admin: Dashboard thống kê nhanh
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.Total = await _db.Orders.CountAsync();
            ViewBag.Pending = await _db.Orders.CountAsync(o => o.Status == "Pending");
            ViewBag.Completed = await _db.Orders.CountAsync(o => o.Status == "Completed");
            ViewBag.Cancelled = await _db.Orders.CountAsync(o => o.Status == "Cancelled");

            return View();
        }

        // ✅ User: hiển thị giỏ hàng trước khi thanh toán
        public async Task<IActionResult> Checkout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cartItems = await _db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống, không thể thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            return View(cartItems);
        }

        // ✅ User: xác nhận thanh toán → tạo đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutConfirm()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cartItems = await _db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                OrderDetails = cartItems.Select(c => new OrderDetail
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product.Price
                }).ToList()
            };

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cartItems); // 🧹 Xóa giỏ hàng sau khi đặt
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ✅ Admin cập nhật trạng thái đơn hàng
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            _db.Update(order);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(AdminIndex));
        }

        // ✅ User/Admin: xem chi tiết đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") && order.UserId.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            return View(order);
        }

        // 👨‍💻 Admin: xem chi tiết tất cả đơn hàng
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDetails(int id)
        {
            var order = await _db.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        // ✅ Admin: chỉnh sửa đơn hàng
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _db.Update(model);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(AdminIndex));
            }
            return View(model);
        }

        // ✅ Admin: xóa đơn hàng
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order != null)
            {
                _db.Orders.Remove(order);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AdminIndex));
        }
    }
}
