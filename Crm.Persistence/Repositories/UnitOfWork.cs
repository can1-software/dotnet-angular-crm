using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using Crm.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Crm.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly CrmDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(CrmDbContext context)
    {
        _context = context;
        Companies = new GenericRepository<Company>(context);
    }

    public IGenericRepository<Company> Companies { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("No transaction has been started.");

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("No transaction has been started.");

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        GC.SuppressFinalize(this);
    }
}
