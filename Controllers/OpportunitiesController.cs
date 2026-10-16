using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;
[Authorize]
public class OpportunitiesController : Controller
{
    private readonly ApplicationDbContext _db;
    public OpportunitiesController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index() => View(await _db.Opportunities.OrderByDescending(x => x.CreatedDate).ToListAsync());
    public async Task<IActionResult> Create() { ViewBag.Customers = await _db.Customers.OrderBy(x => x.CustomerName).ToListAsync(); return View(new Opportunity()); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Opportunity model) { if (model.Status == "Open" && model.ExpectedCloseDate.Date < DateTime.UtcNow.Date) ModelState.AddModelError(nameof(model.ExpectedCloseDate), "Expected Close Date cannot be in the past."); if (!await _db.Customers.AnyAsync(x => x.CustomerId == model.CustomerId)) ModelState.AddModelError(nameof(model.CustomerId), "Customer does not exist."); if (!ModelState.IsValid) { ViewBag.Customers = await _db.Customers.ToListAsync(); return View(model); } model.AssignedTo = User.Identity?.Name; _db.Add(model); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
}
