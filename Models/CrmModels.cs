using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public abstract class OwnedRecord
{
    public string? AssignedTo { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public class Customer : OwnedRecord
{
    public int CustomerId { get; set; }
    [Required, StringLength(120)] public string CustomerCode { get; set; } = "";
    [Required, StringLength(160)] public string CustomerName { get; set; } = "";
    [Required, EmailAddress(ErrorMessage = "Enter a valid email address."), StringLength(200)] public string Email { get; set; } = "";
    [Required, Phone(ErrorMessage = "Enter a valid phone number."), StringLength(30)] public string Phone { get; set; } = "";
    [StringLength(160)] public string? CompanyName { get; set; }
    [StringLength(300)] public string? Address { get; set; }
    [StringLength(80)] public string? City { get; set; }
    [StringLength(80)] public string? State { get; set; }
    [Required] public string Status { get; set; } = "Active";
    public string? CreatedBy { get; set; }
}

public class Lead : OwnedRecord
{
    public int LeadId { get; set; }
    [Required] public string LeadCode { get; set; } = "";
    [Required, StringLength(160)] public string LeadName { get; set; } = "";
    [Required, EmailAddress(ErrorMessage = "Enter a valid email address.")] public string Email { get; set; } = "";
    [Required, Phone(ErrorMessage = "Enter a valid phone number.")] public string Phone { get; set; } = "";
    public string? CompanyName { get; set; }
    public string Source { get; set; } = "Website";
    public string Status { get; set; } = "New";
    [Range(0, double.MaxValue)] public decimal ExpectedValue { get; set; }
    public int? CustomerId { get; set; }
    public bool IsConverted => Status == "Converted";
}

public class Opportunity : OwnedRecord
{
    public int OpportunityId { get; set; }
    [Required, StringLength(160)] public string OpportunityName { get; set; } = "";
    [Required] public int CustomerId { get; set; }
    public int? LeadId { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Opportunity Amount must be greater than 0.")] public decimal Amount { get; set; }
    [Required] public string Stage { get; set; } = "Qualification";
    [Range(0, 100, ErrorMessage = "Probability must be between 0 and 100.")] public int Probability { get; set; }
    [DataType(DataType.Date)] public DateTime ExpectedCloseDate { get; set; } = DateTime.UtcNow.Date.AddDays(30);
    public string Status { get; set; } = "Open";
    public decimal WeightedValue => Amount * Probability / 100m;
}

public class FollowUp : OwnedRecord
{
    public int FollowUpId { get; set; }
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    public int? OpportunityId { get; set; }
    [DataType(DataType.Date)] public DateTime FollowUpDate { get; set; } = DateTime.UtcNow.Date;
    [Required] public string FollowUpType { get; set; } = "Call";
    public string? Remarks { get; set; }
    public string Status { get; set; } = "Planned";
}

public class Activity : OwnedRecord
{
    public int ActivityId { get; set; }
    [Required] public string ActivityType { get; set; } = "Call";
    [Required, StringLength(160)] public string Subject { get; set; } = "";
    public string? Description { get; set; }
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    public string Status { get; set; } = "Planned";
}

public class AuditLog
{
    public long AuditLogId { get; set; }
    public string? UserId { get; set; }
    [Required] public string Action { get; set; } = "";
    [Required] public string EntityName { get; set; } = "";
    public string? RecordId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string Result { get; set; } = "Success";
}
