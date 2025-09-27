using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FurnitureShop.Data;
using FurnitureShop.Models;
using FurnitureShop.ViewModels;

namespace FurnitureShop.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    public AccountController(AppDbContext db) { _db = db; }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login() => View(new LoginVm());

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = _db.Users.FirstOrDefault(u => u.UserName == vm.UserName);
        if (user == null || !user.Verify(vm.Password))
        {
            ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu.");
            return View(vm);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();

        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register() => View(new RegisterVm());

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (_db.Users.Any(u => u.UserName == vm.UserName))
        {
            ModelState.AddModelError("", "Tên tài khoản đã tồn tại.");
            return View(vm);
        }

        var newUser = AppUser.Create(vm.UserName, vm.Password, "User");
        _db.Users.Add(newUser);
        _db.SaveChanges();

        return RedirectToAction("Login");
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
        return RedirectToAction("Login");
    }

    // GET: /Account/Denied
    [HttpGet]
    public IActionResult Denied() => View();
}
