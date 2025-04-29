using System.Text;
using BloodBank.Infrastructure.MessageBus.Interfaces;
using BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;
using Polly.CircuitBreaker;
using RabbitMQ.Client;

namespace BloodBank.Infrastructure.MessageBus;

public class RabbitMqClient : IMessageBusClient, IAsyncDisposable
{
    private readonly ITopologyContext _topologyContext;
    private readonly RabbitMqChannelPool _channelPool;
    private bool _disposed;
    
    public RabbitMqClient(ITopologyContext topologyContext, RabbitMqChannelPool channelPool)
    {
        _topologyContext = topologyContext;
        _channelPool = channelPool;
    }
    

    public async Task Publish(string routingKey, string payload, string exchange)
    {
        var channel = await _channelPool.AcquireChannelAsync();

        try
        {
            if (!_topologyContext.ExchangeExists(exchange))
                throw new ArgumentException($"Exchange '{exchange}' not configured");


            var byteArray = Encoding.UTF8.GetBytes(payload);
            await channel.BasicPublishAsync(exchange, routingKey, byteArray, CancellationToken.None);
        }
        finally
        {
            _channelPool.ReleaseChannel(channel);
        }
      
        
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
    }
}