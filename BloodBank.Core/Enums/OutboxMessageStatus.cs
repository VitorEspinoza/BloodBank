namespace BloodBank.Core.Enums;

public enum OutboxMessageStatus
{
    Pending,
    Processing,
    Processed,
    Failed,
    Archived,
    DeadLettered
}