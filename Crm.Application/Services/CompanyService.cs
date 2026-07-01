using Crm.Application.DTOs.Company;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;

namespace Crm.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<CompanyResponse>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<CompanyResponse?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> CreateAsync(CreateCompanyRequest request)
    {
        var company = new Company
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Industry = request.Industry,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Companies.AddAsync(company);
        await _unitOfWork.SaveChangesAsync();

        return company.Id;
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
