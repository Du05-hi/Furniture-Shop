using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopNoiThat.Data;
using ShopNoiThat.Models;

namespace ShopNoiThat.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ Admin mới được vào
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ShopNoiThatDbContext _context;

        public AdminController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ShopNoiThatDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Trang dashboard
        public async Task<IActionResult> Index()
        {
            var totalUsers = _userManager.Users.Count();
            var totalAdmins = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
            var totalProducts = _context.Products.Count();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalAdmins = totalAdmins;
            ViewBag.TotalProducts = totalProducts;

            return View();
        }

        // Quản lý user
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Xoá user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Users");
        }

        // Thêm user vào role
        [HttpPost]
        public async Task<IActionResult> AddToRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            return RedirectToAction("Users");
        }
    }
}
