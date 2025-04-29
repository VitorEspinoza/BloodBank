using BloodBank.Core.Entities;

namespace BloodBank.Core.Repositories;

public interface IProcessedEventRespository
{
    Task<bool> ExistsAsync(int eventId, string consumerName, CancellationToken cancellationToken  = default);
    
    Task MarkAsSuccessfulAsync(ProcessedEvent processedEvent, CancellationToken cancellationToken = default,  bool saveImmediately = false);
    
    // Task RemoveAsync(int eventId, CancellationToken cancellationToken = default);
}