using System.Text.Json;
using AcxiomCRM.Data;
using AcxiomCRM.Models;

namespace AcxiomCRM.Services;

public class AuditService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;
    public AuditService(ApplicationDbContext db, IHttpContextAccessor http) { _db = db; _http = http; }
    public async Task LogAsync(string action, string entity, string? id, object? oldValue = null, object? newValue = null, string result = "Success")
    {
        _db.AuditLogs.Add(new AuditLog { UserId = _http.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            Action = action, EntityName = entity, RecordId = id, OldValue = oldValue == null ? null : JsonSerializer.Serialize(oldValue),
            NewValue = newValue == null ? null : JsonSerializer.Serialize(newValue), IpAddress = _http.HttpContext?.Connection.RemoteIpAddress?.ToString(), Result = result });
        await _db.SaveChangesAsync();
    }
}
