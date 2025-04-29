using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Persistence.Repositories;

public class ProcessedEventRepository : IProcessedEventRespository
{
    private readonly BloodBankDbContext _context;

    public ProcessedEventRepository(BloodBankDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> ExistsAsync(int eventId, string consumerName, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessedEvents.AnyAsync(e => e.EventId == eventId && e.ConsumerName == consumerName,  cancellationToken);
    }

    public async Task MarkAsSuccessfulAsync(ProcessedEvent processedEvent, CancellationToken cancellationToken = default, bool saveImmediately = false)
    {
        await _context.ProcessedEvents.AddAsync(processedEvent, cancellationToken);
        
        if(saveImmediately)
            await _context.SaveChangesAsync(cancellationToken);
    }
}