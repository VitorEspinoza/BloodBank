namespace BloodBank.Infrastructure.MessageBus;

public class OutboxSettings
{
    public int BatchSize { get; set; } = 50;
    
    public int ImmediateRetryCount { get; set; } = 3;
    
    public int MessageRetryLimit { get; set; } = 5;
    public int ProcessingIntervalSeconds { get; set; } = 10;
    public int MessageLockTimeoutSeconds { get; set; } = 60;
    public int CleanupIntervalHours { get; set; } = 24;
    public int MessageRetentionDays { get; set; } = 7;
    public int CircuitBreakerThreshold { get; set; } = 10;
    public int CircuitBreakerResetSeconds { get; set; } = 60;
}