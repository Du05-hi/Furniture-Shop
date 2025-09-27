using FurnitureShop.Data;
using FurnitureShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin/users/{action=Index}/{id?}")]
public class AdminUsersController : Controller
{
    private readonly AppDbContext _db;
    public AdminUsersController(AppDbContext db) { _db = db; }

    public async Task<IActionResult> Index()
    {
        var users = await _db.Users.AsNoTracking().OrderBy(u => u.UserName).ToListAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Promote(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null) { user.Role = "Admin"; await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Demote(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null) { user.Role = "User"; await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null) { _db.Users.Remove(user); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}