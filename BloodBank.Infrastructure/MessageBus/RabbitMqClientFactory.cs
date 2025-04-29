using BloodBank.Infrastructure.MessageBus.Interfaces;
using BloodBank.Infrastructure.MessageBus.TopologyConfig;
using BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;
using Microsoft.Extensions.Options;
using Polly;

namespace BloodBank.Infrastructure.MessageBus;

public class RabbitMqClientFactory
{
    
    public static async Task<IMessageBusClient> CreateClientAsync(ITopologyContext topologyContext, IEventBusTopologyDefinition topologyDefinition, RabbitMqChannelPool channelPool)
    {
        var channel = await channelPool.AcquireChannelAsync();
        
        var topologyInitializer = new RabbitMqTopologyInitializer(channel, topologyContext, topologyDefinition);
        await topologyInitializer.InitializeTopologyAsync();
        
        return new RabbitMqClient(topologyContext, channelPool);
    }
}