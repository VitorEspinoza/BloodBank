using BloodBank.Core.Enums;

namespace BloodBank.Core.Entities;

public class OutboxMessage
{
    public int Id { get; protected set; }
    public string EventType { get; private set; }
    public string Payload { get; private set; }
    public string Exchange { get; private set; }
    public string RoutingKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }
    public OutboxMessageStatus Status { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    private OutboxMessage() { } 
    
    public static OutboxMessage Create(
        string eventType,
        string payload,
        string exchange,
        string routingKey
    )
    {
        return new OutboxMessage
        {
            EventType = eventType,
            Payload = payload,
            Exchange = exchange,
            RoutingKey = routingKey,
            CreatedAt = DateTime.UtcNow,
            Status = OutboxMessageStatus.Pending,
            RetryCount = 0
        };
    }

    public void MarkAsProcessed()
    {
        Status = OutboxMessageStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        LockedUntil = null;
    }

    public void MarkAsFailed(string error)
    {
        Status = OutboxMessageStatus.Failed;
        Error = error;
        RetryCount++;
        LockedUntil = null;
    }

    public void AcquireLock(TimeSpan duration)
    {
        Status = OutboxMessageStatus.Processing;
        LockedUntil = DateTime.UtcNow.Add(duration);
    }

    public bool ShouldRetry(int maxRetries)
    {
        return RetryCount < maxRetries;
    }

    public void ResetForRetry()
    {
        Status = OutboxMessageStatus.Pending;
        LockedUntil = DateTime.UtcNow.Add(CalculateBackoff());
    }

    public TimeSpan CalculateBackoff()
    {
        var exponent = Math.Min(RetryCount, 10);
        return TimeSpan.FromSeconds(Math.Pow(2, exponent));
    }
    
    public void MarkAsMovedToDlx()
    {
        Status = OutboxMessageStatus.DeadLettered;
    }
    
    public void UpdateError(string error)
    {
        Error = error;
    }
}

