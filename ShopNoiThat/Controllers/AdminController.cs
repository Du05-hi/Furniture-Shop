using Microsoft.AspNetCore.Mvc;
using ShopNoiThat.Models;

namespace ShopNoiThat.Controllers
{
    public class AdminController : Controller
    {
        private readonly ShopNoiThatDbContext _context;

        public AdminController(ShopNoiThatDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Auth");

            ViewBag.TotalProducts = _context.Products.Count();
            ViewBag.TotalUsers = _context.Users.Count();
            return View();
        }

        public IActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Auth");
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult DeleteUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Auth");
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Users");
        }
    }
}
