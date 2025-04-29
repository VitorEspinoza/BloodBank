using RabbitMQ.Client;

namespace BloodBank.Infrastructure.MessageBus.TopologyConfig.ConfigsDefinition;

public class ExchangeConfig
{
    public string Type { get; set; } = ExchangeType.Topic;
    public bool Durable { get; set; } = true;
    public bool AutoDelete { get; set; } = false;
    public IDictionary<string, object> Arguments { get; set; }
    
}