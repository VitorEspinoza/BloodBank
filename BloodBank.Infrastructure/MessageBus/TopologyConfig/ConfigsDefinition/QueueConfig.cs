namespace BloodBank.Infrastructure.MessageBus.TopologyConfig.ConfigsDefinition;

public class QueueConfig
{
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public IDictionary<string, object> Arguments { get; set; }
    public string RoutingKey { get; set; }
    public string Exchange { get; set; }
}
