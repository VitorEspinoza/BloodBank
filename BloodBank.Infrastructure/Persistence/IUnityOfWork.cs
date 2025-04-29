using BloodBank.Core.Repositories;

namespace BloodBank.Infrastructure.Persistence;

public interface IUnitOfWork : IDisposable
{
    Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action, 
        CancellationToken cancellationToken = default
    );
}