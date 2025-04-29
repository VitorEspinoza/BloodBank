using System.Text;
using System.Text.Json;
using BloodBank.Core.DomainEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BloodBank.Application.Subscribers;

public abstract class BaseSubscriber<TDomainEvent> : BackgroundService, IAsyncDisposable where TDomainEvent : IDomainEvent
{
    private readonly IServiceProvider _serviceProvider;
    private IConnection _connection;
    private IChannel _channel;
    private readonly string _queueName;
    private bool _disposed;

    protected BaseSubscriber(
        IServiceProvider serviceProvider,
        string queueName)
    {
        _serviceProvider = serviceProvider;
        _queueName = queueName;
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await InitializeConnectionAsync();
        await base.StartAsync(cancellationToken);
    }
    
    private async Task InitializeConnectionAsync()
    {
       using var scope = _serviceProvider.CreateScope();
       var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var factory = new ConnectionFactory { 
            HostName = config["MessageBusSettings:HostName"] ?? "localhost",
            Port = int.Parse(config["MessageBusSettings:Port"] ?? "5672"),
            UserName = config["MessageBusSettings:UserName"] ?? "guest",
            Password = config["MessageBusSettings:Password"] ?? "guest"
        };
        _connection = await factory.CreateConnectionAsync($"{typeof(TDomainEvent).Name}-subscriber");
        _channel = await _connection.CreateChannelAsync();
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, args) => 
            await HandleMessageAsync(args, cancellationToken);
    
        await _channel.BasicConsumeAsync(_queueName, false, consumer, cancellationToken);
    
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
    
    private async Task HandleMessageAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;
        
        try
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());
        
            var message = JsonSerializer.Deserialize<TDomainEvent>(body);
        
            if (message == null)
            {
                await _channel.BasicNackAsync(args.DeliveryTag, false, true, cancellationToken);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var result = await ProcessEventAsync(message, scope.ServiceProvider, cancellationToken);

            if (result)
            {
                await _channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
            }
            else
            {
                await _channel.BasicNackAsync(args.DeliveryTag, false, true, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            await _channel.BasicNackAsync(args.DeliveryTag, false, true, cancellationToken);
        }
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
    protected abstract Task<bool> ProcessEventAsync(TDomainEvent @event, IServiceProvider serviceProvider, CancellationToken cancellationToken);
    
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_channel?.IsOpen ?? false)
            await _channel.CloseAsync();
    
        if (_connection?.IsOpen ?? false)
            await _connection.CloseAsync();
        
        _channel?.Dispose();
        _connection?.Dispose();
        
        GC.SuppressFinalize(this);
    }
}