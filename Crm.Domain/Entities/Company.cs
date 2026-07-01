using Crm.Domain.Common;

namespace Crm.Domain.Entities;

public class Company : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Industry { get; set; }
    public bool IsActive { get; set; } = true;
}