using Crm.Application.DTOs.Company;
using Crm.Application.Interfaces;

namespace Crm.Application.Services;

public class CompanyService : ICompanyService
{
    public CompanyService()
    {
    }

    public Task<List<CompanyResponse>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<CompanyResponse?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> CreateAsync(CreateCompanyRequest request)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(UpdateCompanyRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
