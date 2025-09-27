using FurnitureShop.Data;
using FurnitureShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin/products/{action=Index}/{id?}")]
public class AdminProductsController : Controller
{
    private readonly AppDbContext _db;
    public AdminProductsController(AppDbContext db) { _db = db; }

    public async Task<IActionResult> Index()
    {
        var items = await _db.Products.AsNoTracking().OrderByDescending(p => p.Id).ToListAsync();
        return View(items);
    }

    [HttpGet]
    public IActionResult Create() => View(new Product());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Products.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.Products.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Products.Update(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Products.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Products.FindAsync(id);
        if (item != null) { _db.Products.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}