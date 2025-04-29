namespace BloodBank.Infrastructure.MessageBus.Interfaces;

public interface IMessageBusClient
{
    Task Publish(string routingKey, string payload, string exchange);
}