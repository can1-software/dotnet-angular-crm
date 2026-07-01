namespace Crm.Application.DTOs.Company;

public class CompanyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Industry { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
