using Microsoft.EntityFrameworkCore.Storage;

namespace BloodBank.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    
    private IDbContextTransaction _transaction;
    private readonly BloodBankDbContext _context;

    public UnitOfWork(BloodBankDbContext context)
    {
        _context = context;
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action, 
        CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action();
            await CommitAsync(cancellationToken); 
            return result;
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
    }
    private async Task BeginTransactionAsync(CancellationToken ct)
    {
        _transaction = await _context.Database.BeginTransactionAsync(ct);
    }
    
    private async Task CommitAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
        await _transaction.CommitAsync(ct);
    }
    private async Task RollbackAsync(CancellationToken ct)
    {
        await _transaction.RollbackAsync(ct);
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
}