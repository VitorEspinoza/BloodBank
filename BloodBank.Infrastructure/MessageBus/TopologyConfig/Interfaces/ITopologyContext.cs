using BloodBank.Infrastructure.MessageBus.TopologyConfig.ConfigsDefinition;

namespace BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;

public interface ITopologyContext
{
    bool ExchangeExists(string exchangeName);
    ExchangeConfig GetExchangeConfig(string exchangeName);
    IEnumerable<string> GetConfiguredExchangeNames();
    
    IEnumerable<string> GetConfiguredQueueNames();
    
    QueueConfig GetQueueConfig(string queueName);
}