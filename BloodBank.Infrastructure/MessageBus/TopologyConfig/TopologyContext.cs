using BloodBank.Infrastructure.MessageBus.TopologyConfig.ConfigsDefinition;
using BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;
using Microsoft.Extensions.Options;

namespace BloodBank.Infrastructure.MessageBus.TopologyConfig;

public class TopologyContext : ITopologyContext
{
    private readonly MessageBusSettings _settings;

    public TopologyContext(IOptions<MessageBusSettings> settings)
    {
       _settings = settings.Value;
    }

    public bool ExchangeExists(string exchangeName) 
        => _settings.Exchanges.ContainsKey(exchangeName);

    public IEnumerable<string> GetConfiguredExchangeNames() 
        => _settings.Exchanges?.Keys ?? Enumerable.Empty<string>();

    public ExchangeConfig GetExchangeConfig(string exchangeName) 
        => _settings.Exchanges[exchangeName];

    public IEnumerable<string> GetConfiguredQueueNames() 
        => _settings.Queues?.Keys ?? Enumerable.Empty<string>();

    public QueueConfig GetQueueConfig(string queueName) 
        => _settings.Queues[queueName];
}