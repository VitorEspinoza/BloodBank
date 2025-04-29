
using DotNet.Testcontainers.Builders;
using Testcontainers.RabbitMq;

namespace BloodBank.Testing.IntegrationTests.Infrastructure.Containers;

public static class RabbitMqTestContainer
{
    public static RabbitMqContainer CreateContainer()
    {
        return new RabbitMqBuilder()
            .WithImage("rabbitmq:4-management") 
            .WithPortBinding(5672) 
            .WithPortBinding(15672) 
            .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
            .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest")
            .WithEnvironment("RABBITMQ_MANAGEMENT_INTERFACE", "0.0.0.0")
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilPortIsAvailable(5672)
                 
            )
            .Build();
    }

}