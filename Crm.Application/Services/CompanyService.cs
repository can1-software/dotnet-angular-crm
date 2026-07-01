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

    public async Task<List<CompanyResponse>> GetAllAsync()
    {
        var companies = await _unitOfWork.Companies.GetAllAsync();
        return companies.Select(MapToResponse).ToList();
    }

    public async Task<CompanyResponse?> GetByIdAsync(Guid id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company is null)
            return null;

        return MapToResponse(company);
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

    public async Task UpdateAsync(UpdateCompanyRequest request)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(request.Id);
        if (company is null)
            throw new KeyNotFoundException($"Company with id '{request.Id}' was not found.");

        company.Name = request.Name;
        company.Email = request.Email;
        company.Phone = request.Phone;
        company.Industry = request.Industry;
        company.IsActive = request.IsActive;
        company.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Companies.Update(company);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company is null)
            throw new KeyNotFoundException($"Company with id '{id}' was not found.");

        _unitOfWork.Companies.Delete(company);
        await _unitOfWork.SaveChangesAsync();
    }

    private static CompanyResponse MapToResponse(Company company) => new()
    {
        Id = company.Id,
        Name = company.Name,
        Email = company.Email,
        Phone = company.Phone,
        Industry = company.Industry,
        IsActive = company.IsActive,
        CreatedAt = company.CreatedAt
    };
}
