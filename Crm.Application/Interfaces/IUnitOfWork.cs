using Crm.Domain.Entities;

namespace Crm.Application.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<Company> Companies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
