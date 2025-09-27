using FurnitureShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Controllers;

public class ProductsController : Controller
{
    private readonly AppDbContext _db;
    public ProductsController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var list = await _db.Products.AsNoTracking().OrderByDescending(p => p.Id).ToListAsync();
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var item = await _db.Products.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }
}