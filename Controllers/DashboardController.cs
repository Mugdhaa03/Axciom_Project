using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    public DashboardController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index()
    {
        var open = await _db.Opportunities.Where(x => x.Status == "Open").ToListAsync();
        ViewBag.Customers = await _db.Customers.CountAsync();
        ViewBag.Leads = await _db.Leads.CountAsync();
        ViewBag.OpenLeads = await _db.Leads.CountAsync(x => x.Status != "Converted" && x.Status != "Lost");
        ViewBag.Opportunities = await _db.Opportunities.CountAsync();
        ViewBag.OpenOpportunities = open.Count;
        ViewBag.Won = await _db.Opportunities.CountAsync(x => x.Status == "Won");
        ViewBag.Lost = await _db.Opportunities.CountAsync(x => x.Status == "Lost");
        ViewBag.Pipeline = open.Sum(x => x.Amount);
        ViewBag.Weighted = open.Sum(x => x.WeightedValue);
        ViewBag.LeadChart = await _db.Leads.GroupBy(x => x.Status).Select(x => new { Label = x.Key, Count = x.Count() }).ToListAsync();
        var opportunities = await _db.Opportunities.AsNoTracking().ToListAsync();
        ViewBag.OpportunityChart = opportunities.GroupBy(x => x.Stage)
            .Select(x => new { Label = x.Key, Count = x.Count(), Amount = x.Sum(y => y.Amount) })
            .ToList();
        return View();
    }
}
