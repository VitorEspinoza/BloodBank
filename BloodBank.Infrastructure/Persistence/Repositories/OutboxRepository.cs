using System.Text.Json;
using BloodBank.Core.DomainEvents;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.MessageBus.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Persistence.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly BloodBankDbContext _context;
    private readonly IEventExchangeResolver _exchangeResolver;

    public OutboxRepository(BloodBankDbContext context, IEventExchangeResolver exchangeResolver)
    {
        _context = context;
        _exchangeResolver = exchangeResolver;
    }
    
    public async Task SaveEvent(IDomainEvent @event) 
    {
        var routingKey = @event.GetType().Name.ToDashCase();
        await SaveEventWithRoutingKey(@event, routingKey);
    }

    public async Task SaveEventWithRoutingKey(IDomainEvent @event, string routingKey)
    {
        var exchangeName = _exchangeResolver.Resolve(@event);
        var payload = JsonSerializer.Serialize(@event, @event.GetType());
        
        var outboxMessage = OutboxMessage.Create(
            @event.GetType().FullName,
            payload,
            exchangeName,
            routingKey);

        await AddAsync(outboxMessage);
    }

    public async Task AddAsync(OutboxMessage outboxMessage)
    {
        await _context.OutboxMessages.AddAsync(outboxMessage);
    }

    public async Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _context.OutboxMessages
            .AsNoTracking()
            .Where(m =>
                m.Status == OutboxMessageStatus.Pending && (m.LockedUntil == null || m.LockedUntil < now))
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default)
    {
        _context.OutboxMessages.Update(outboxMessage);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken = default)
    {
        _context.OutboxMessages.UpdateRange(messages);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> ArchiveProcessedMessagesAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Processed && m.ProcessedAt < cutoffDate)
            .ExecuteUpdateAsync(m => m.SetProperty(p => p.Status, OutboxMessageStatus.Archived), cancellationToken);
    }
}