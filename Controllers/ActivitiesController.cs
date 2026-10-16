using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;
[Authorize]
public class ActivitiesController : Controller
{
    private readonly ApplicationDbContext _db;
    public ActivitiesController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index(string? type, string? status) => View(await _db.Activities.Where(x => (type == null || x.ActivityType == type) && (status == null || x.Status == status)).OrderByDescending(x => x.ActivityDate).ToListAsync());
    public IActionResult Create() => View(new Activity());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Activity model) { if (!ModelState.IsValid) return View(model); model.AssignedTo = User.Identity?.Name; _db.Add(model); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
}
