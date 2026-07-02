using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Crm.Persistence.Context;

public class CrmDbContextFactory : IDesignTimeDbContextFactory<CrmDbContext>
{
    public CrmDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CrmDbContext>();
        var connectionString = "Server=localhost,1433;Database=CrmDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(connectionString);

        return new CrmDbContext(optionsBuilder.Options);
    }
}
