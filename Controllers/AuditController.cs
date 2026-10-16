using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;
[Authorize(Roles = "Admin")]
public class AuditController : Controller
{
    private readonly ApplicationDbContext _db;
    public AuditController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index(string? actionName) => View(await _db.AuditLogs.Where(x => actionName == null || x.Action == actionName).OrderByDescending(x => x.CreatedDate).Take(200).ToListAsync());
}
