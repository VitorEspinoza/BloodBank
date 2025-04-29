using RabbitMQ.Client;

namespace BloodBank.Infrastructure.MessageBus;

public class ProducerConnection
{
    public ProducerConnection(IConnection connection)
    {
        Connection = connection;
    }

    public IConnection Connection { get; }
}