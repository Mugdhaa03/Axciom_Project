using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;
[Authorize]
public class LeadsController : Controller
{
    private readonly ApplicationDbContext _db;
    public LeadsController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index(string? search, string? status) => View(await _db.Leads.Where(x => (search == null || x.LeadName.Contains(search) || (x.CompanyName != null && x.CompanyName.Contains(search))) && (status == null || x.Status == status)).OrderByDescending(x => x.CreatedDate).ToListAsync());
    public IActionResult Create() => View(new Lead());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lead model) { if (!ModelState.IsValid) return View(model); model.LeadCode = "LED-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(); model.AssignedTo = User.Identity?.Name; _db.Add(model); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
}
