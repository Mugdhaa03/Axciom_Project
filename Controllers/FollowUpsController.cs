using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;
[Authorize]
public class FollowUpsController : Controller
{
    private readonly ApplicationDbContext _db;
    public FollowUpsController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index(string? status) => View(await _db.FollowUps.Where(x => status == null || x.Status == status).OrderBy(x => x.FollowUpDate).ToListAsync());
    public async Task<IActionResult> Create()
    {
        ViewBag.Customers = await _db.Customers.OrderBy(x => x.CustomerName).ToListAsync();
        return View(new FollowUp());
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FollowUp model)
    {
        if ((model.Status == "Planned" || model.Status == "New") && model.FollowUpDate.Date < DateTime.UtcNow.Date)
            ModelState.AddModelError(nameof(model.FollowUpDate), "Follow-up date cannot be earlier than today.");
        if (!ModelState.IsValid) { ViewBag.Customers = await _db.Customers.ToListAsync(); return View(model); }
        model.AssignedTo = User.Identity?.Name; _db.Add(model); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id) { var x = await _db.FollowUps.FindAsync(id); if (x == null) return NotFound(); x.Status = "Completed"; await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
}
