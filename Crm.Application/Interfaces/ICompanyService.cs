using Crm.Application.DTOs.Company;

namespace Crm.Application.Interfaces;

public interface ICompanyService
{
    Task<List<CompanyResponse>> GetAllAsync();
    Task<CompanyResponse?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateCompanyRequest request);
    Task UpdateAsync(UpdateCompanyRequest request);
    Task DeleteAsync(Guid id);
}
