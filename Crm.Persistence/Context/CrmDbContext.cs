using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crm.Persistence.Context;

public class CrmDbContext : DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
}