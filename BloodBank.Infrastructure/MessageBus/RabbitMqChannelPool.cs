using System.Collections.Concurrent;
using RabbitMQ.Client;

namespace BloodBank.Infrastructure.MessageBus;

public class RabbitMqChannelPool : IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly ConcurrentBag<IChannel> _pool = new();
    private readonly SemaphoreSlim _semaphore;

    public RabbitMqChannelPool(IConnection connection, int maxPoolSize = 10)
    {
        _connection = connection;
        _semaphore = new SemaphoreSlim(maxPoolSize);
    }

    public async Task<IChannel> AcquireChannelAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_pool.TryTake(out var channel) && !channel.IsClosed)
                return channel;

            return await _connection.CreateChannelAsync();
        }
        catch
        {
            _semaphore.Release();
            throw;
        }
  
    }

    public void ReleaseChannel(IChannel channel)
    {
        if(channel.IsClosed)
            channel.Dispose();
        else
            _pool.Add(channel);
        
        _semaphore.Release();
    }
    public async ValueTask DisposeAsync()
    {
        foreach (var channel in  _pool)
        {
            if(channel.IsOpen)
                await channel.CloseAsync();
            channel.Dispose();
        }
        _semaphore.Dispose();
    }
}