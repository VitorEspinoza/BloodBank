using BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;
using RabbitMQ.Client;

namespace BloodBank.Infrastructure.MessageBus.TopologyConfig;

public class RabbitMqTopologyInitializer
{
    private readonly IChannel _channel;
    private readonly ITopologyContext _topologyContext;
    private readonly IEventBusTopologyDefinition _topologyDefinition;
    
    public RabbitMqTopologyInitializer(
        IChannel channel, 
        ITopologyContext topologyContext,
        IEventBusTopologyDefinition topologyDefinition)
    {
        _channel = channel;
        _topologyContext = topologyContext;
        _topologyDefinition = topologyDefinition;
    }
    
    public async Task InitializeTopologyAsync()
    {

        await InitializeDlxAsync();
        await InitializeExchangesAsync();
        await InitializeQueuesAsync();
    }

    private async Task InitializeDlxAsync()
    {
        await _channel.ExchangeDeclareAsync(
            exchange: _topologyDefinition.DeadLetterExchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);
            
        await _channel.QueueDeclareAsync(
            queue: _topologyDefinition.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);
            
        await _channel.QueueBindAsync(
            queue: _topologyDefinition.DeadLetterQueue,
            exchange: _topologyDefinition.DeadLetterExchange,
            routingKey: "#");
    }

    private async Task InitializeExchangesAsync()
    {
        foreach (var exchangeName in _topologyContext.GetConfiguredExchangeNames()!)
        {
            var config = _topologyContext.GetExchangeConfig(exchangeName);
            
            var arguments = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _topologyDefinition.DeadLetterExchange }
            };

            if (config.Arguments != null)
            {
                foreach (var arg in config.Arguments)
                {
                    arguments[arg.Key] = arg.Value;
                }
            }
            
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: config.Type,
                durable: config.Durable,
                autoDelete: false);
        }
    }

    private async Task InitializeQueuesAsync()
    {
        foreach (var queueName in _topologyContext.GetConfiguredQueueNames())
        {
            var queueConfig = _topologyContext.GetQueueConfig(queueName);
        
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: queueConfig.Durable,
                exclusive: queueConfig.Exclusive,
                autoDelete: queueConfig.AutoDelete,
                arguments: queueConfig.Arguments);

            if (!string.IsNullOrEmpty(queueConfig.Exchange))
            {
                await _channel.QueueBindAsync(
                    queue: queueName,
                    exchange: queueConfig.Exchange,
                    routingKey: queueConfig.RoutingKey);
            }
        }
    }
}