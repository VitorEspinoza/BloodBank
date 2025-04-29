using BloodBank.Core.DomainEvents;
using BloodBank.Core.Entities;

namespace BloodBank.Core.Repositories;

public interface IOutboxRepository
{
    Task SaveEvent(IDomainEvent @event);
    Task SaveEventWithRoutingKey(IDomainEvent @event, string routingKey);
    Task AddAsync(OutboxMessage outboxMessage);
    Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize, CancellationToken cancellationToken = default);
    Task UpdateAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken = default);
    Task<int> ArchiveProcessedMessagesAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
}