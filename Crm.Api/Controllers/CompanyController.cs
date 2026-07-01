using Crm.Application.DTOs.Company;
using Crm.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCompanyRequest request)
    {
        var id = await _companyService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CompanyResponse>> GetById(Guid id)
    {
        var company = await _companyService.GetByIdAsync(id);
        if (company is null)
            return NotFound();

        return Ok(company);
    }
}
