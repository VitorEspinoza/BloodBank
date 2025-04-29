namespace BloodBank.Core.Entities;

public class ProcessedEvent
{
    public int EventId { get; set; }
    public string ConsumerName { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string? EventType { get; set; }
    
    private ProcessedEvent() { }

    public ProcessedEvent(int eventId, string consumerName, string? eventType)
    {
        EventId = eventId;
        ConsumerName = consumerName;
        ProcessedAt = DateTime.UtcNow;
        EventType = eventType;
    }
}