using System.Text;
using System.Text.Json;
using BloodBank.Core.DomainEvents;
using BloodBank.Infrastructure.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BloodBank.Application.Subscribers;

public abstract class BaseSubscriber<TDomainEvent> : BackgroundService where TDomainEvent : IDomainEvent
{
    private readonly IServiceProvider _serviceProvider;
    private IChannel _channel;
    private readonly string _queueName;
    private string _consumerTag;
    private readonly RabbitMqChannelPool _channelPool;

    protected BaseSubscriber(
        IServiceProvider serviceProvider,
        string queueName)
    {
        _serviceProvider = serviceProvider;
        _queueName = queueName;
        _channelPool = serviceProvider.GetRequiredService<RabbitMqChannelPool>();
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {

        _channel = await _channelPool.AcquireChannelAsync();
        await base.StartAsync(cancellationToken);
    }


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, args) =>
            await HandleMessageAsync(args, cancellationToken);

        _consumerTag =  await _channel.BasicConsumeAsync(_queueName, false, consumer, cancellationToken);

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

        if (_channel?.IsOpen == true && !string.IsNullOrEmpty(_consumerTag))
        {
            await _channel.BasicCancelAsync(_consumerTag, cancellationToken: cancellationToken);
        }

        if (_channel != null)
        {
            _channelPool.ReleaseChannel(_channel);
            _channel = null;
        }
        
        await base.StopAsync(cancellationToken);
    }

    protected abstract Task<bool> ProcessEventAsync(TDomainEvent @event, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}
    