using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[ApiController, Route("api")]
[Authorize]
public class ApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public ApiController(ApplicationDbContext db) => _db = db;

    [HttpGet("customers")]
    public async Task<IActionResult> Customers([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 100);
        var query = _db.Customers.AsNoTracking().Where(x => string.IsNullOrEmpty(search) || x.CustomerName.Contains(search) || (x.CompanyName != null && x.CompanyName.Contains(search)));
        return Ok(new { page, pageSize, total = await query.CountAsync(), items = await query.OrderBy(x => x.CustomerName).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new { x.CustomerId, x.CustomerCode, x.CustomerName, x.Email, x.Phone, x.CompanyName, x.Status, x.CreatedDate }).ToListAsync() });
    }
    [HttpGet("customers/{id:int}")]
    public async Task<IActionResult> Customer(int id) { var x = await _db.Customers.AsNoTracking().Where(x => x.CustomerId == id).Select(x => new { x.CustomerId, x.CustomerCode, x.CustomerName, x.Email, x.Phone, x.CompanyName, x.Status, x.CreatedDate }).SingleOrDefaultAsync(); return x == null ? NotFound() : Ok(x); }
    [HttpPost("customers")]
    public async Task<IActionResult> CreateCustomer(Customer model)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (await _db.Customers.AnyAsync(x => x.Email == model.Email || x.Phone == model.Phone)) return Conflict(new { error = "A customer with this email or phone already exists." });
        model.CustomerCode = "CUS-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(); model.CreatedBy = User.Identity?.Name; model.AssignedTo = User.Identity?.Name;
        _db.Customers.Add(model); await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Customer), new { id = model.CustomerId }, new { model.CustomerId, model.CustomerCode, model.CustomerName, model.Email, model.Phone, model.Status });
    }
    [HttpPut("customers/{id:int}")]
    public async Task<IActionResult> UpdateCustomer(int id, Customer input) { var x = await _db.Customers.FindAsync(id); if (x == null) return NotFound(); if (!ModelState.IsValid) return ValidationProblem(ModelState); x.CustomerName = input.CustomerName; x.Email = input.Email; x.Phone = input.Phone; x.CompanyName = input.CompanyName; x.Address = input.Address; x.City = input.City; x.State = input.State; x.Status = input.Status; await _db.SaveChangesAsync(); return Ok(new { x.CustomerId, x.CustomerName, x.Email, x.Phone, x.Status }); }
    [HttpDelete("customers/{id:int}")]
    public async Task<IActionResult> DeleteCustomer(int id) { var x = await _db.Customers.FindAsync(id); if (x == null) return NotFound(); x.Status = "Inactive"; await _db.SaveChangesAsync(); return NoContent(); }

    [HttpGet("leads")]
    public async Task<IActionResult> Leads([FromQuery] string? status) => Ok(await _db.Leads.AsNoTracking().Where(x => status == null || x.Status == status).Select(x => new { x.LeadId, x.LeadCode, x.LeadName, x.Email, x.CompanyName, x.Status, x.ExpectedValue, x.AssignedTo }).ToListAsync());
    [HttpPost("leads")]
    public async Task<IActionResult> CreateLead(Lead model) { if (!ModelState.IsValid) return ValidationProblem(ModelState); model.LeadCode = "LED-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(); model.AssignedTo ??= User.Identity?.Name; _db.Leads.Add(model); await _db.SaveChangesAsync(); return Created($"/api/leads/{model.LeadId}", new { model.LeadId, model.LeadCode, model.LeadName, model.Status }); }
    [HttpGet("opportunities")]
    public async Task<IActionResult> Opportunities([FromQuery] string? stage) => Ok(await _db.Opportunities.AsNoTracking().Where(x => stage == null || x.Stage == stage).Select(x => new { x.OpportunityId, x.OpportunityName, x.CustomerId, x.Amount, x.Stage, x.Probability, x.ExpectedCloseDate, x.Status, x.AssignedTo }).ToListAsync());
    [HttpPost("opportunities")]
    public async Task<IActionResult> CreateOpportunity(Opportunity model)
    {
        if (model.Status == "Open" && model.ExpectedCloseDate.Date < DateTime.UtcNow.Date) ModelState.AddModelError(nameof(model.ExpectedCloseDate), "Expected Close Date cannot be in the past.");
        if (!await _db.Customers.AnyAsync(x => x.CustomerId == model.CustomerId)) ModelState.AddModelError(nameof(model.CustomerId), "Customer does not exist.");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        model.AssignedTo ??= User.Identity?.Name; _db.Opportunities.Add(model); await _db.SaveChangesAsync(); return Created($"/api/opportunities/{model.OpportunityId}", new { model.OpportunityId, model.OpportunityName, model.Amount, model.Stage, model.Status });
    }
}
