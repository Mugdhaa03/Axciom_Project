using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly AuditService _audit;
    public CustomersController(ApplicationDbContext db, AuditService audit) { _db = db; _audit = audit; }
    public async Task<IActionResult> Index(string? search) => View(await _db.Customers.Where(x => string.IsNullOrEmpty(search) || x.CustomerName.Contains(search) || x.CompanyName!.Contains(search)).OrderByDescending(x => x.CreatedDate).ToListAsync());
    public IActionResult Create() => View(new Customer());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer model)
    {
        if (await _db.Customers.AnyAsync(x => x.Email == model.Email || x.Phone == model.Phone)) ModelState.AddModelError("", "A customer with this email or phone already exists.");
        if (!ModelState.IsValid) return View(model);
        model.CustomerCode = "CUS-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        model.CreatedBy = User.Identity?.Name; model.AssignedTo = User.Identity?.Name;
        _db.Customers.Add(model); await _db.SaveChangesAsync(); await _audit.LogAsync("Created", "Customer", model.CustomerId.ToString(), newValue: model);
        TempData["Success"] = "Customer created successfully."; return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Details(int id) { var x = await _db.Customers.FindAsync(id); return x == null ? NotFound() : View(x); }
    public async Task<IActionResult> Edit(int id) { var x = await _db.Customers.FindAsync(id); return x == null ? NotFound() : View(x); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer model) { if (id != model.CustomerId) return BadRequest(); if (!ModelState.IsValid) return View(model); _db.Update(model); await _db.SaveChangesAsync(); await _audit.LogAsync("Updated", "Customer", id.ToString(), newValue: model); return RedirectToAction(nameof(Index)); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id) { var x = await _db.Customers.FindAsync(id); if (x == null) return NotFound(); x.Status = "Inactive"; await _db.SaveChangesAsync(); await _audit.LogAsync("Deactivated", "Customer", id.ToString()); return RedirectToAction(nameof(Index)); }
}
