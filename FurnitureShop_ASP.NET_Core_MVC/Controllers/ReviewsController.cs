using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Data;
using FurnitureShop.Models;
using System.Security.Claims;

[Authorize]
public class ReviewsController : Controller
{
    private readonly AppDbContext _db;
    public ReviewsController(AppDbContext db) => _db = db;

    // GET: /Reviews
    public async Task<IActionResult> Index()
    {
        var reviews = await _db.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return View(reviews);
    }

    // GET: /Reviews/Create
    public IActionResult Create(int productId)
    {
        var model = new Review { ProductId = productId };
        return View(model);
    }

    // POST: /Reviews/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Review model)
    {
        if (ModelState.IsValid)
        {
            model.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.CreatedAt = DateTime.Now;
            _db.Reviews.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }
        return View(model);
    }
}
