namespace BloodBank.Infrastructure.MessageBus.TopologyConfig.ConfigsDefinition;
public class MessageBusSettings
{
    public Dictionary<string, ExchangeConfig> Exchanges { get; set; } = new();
    public Dictionary<string, QueueConfig> Queues { get; set; } = new();
}
