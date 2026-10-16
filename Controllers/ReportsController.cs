using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;
[Authorize(Roles = "Admin,Manager")]
public class ReportsController : Controller
{
    private readonly ApplicationDbContext _db;
    public ReportsController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index()
    {
        ViewBag.Customers = await _db.Customers.GroupBy(x => x.Status).Select(x => new { Label = x.Key, Count = x.Count() }).ToListAsync();
        ViewBag.Leads = await _db.Leads.GroupBy(x => x.Status).Select(x => new { Label = x.Key, Count = x.Count() }).ToListAsync();
        var opportunities = await _db.Opportunities.AsNoTracking().ToListAsync();
        ViewBag.Opportunities = opportunities.GroupBy(x => x.Stage)
            .Select(x => new { Label = x.Key, Amount = x.Sum(y => y.Amount), Count = x.Count() })
            .ToList();
        return View();
    }
}
