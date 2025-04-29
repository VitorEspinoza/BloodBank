
using BloodBank.Infrastructure;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Testing.IntegrationTests.Infrastructure.Containers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Microsoft.Extensions.Hosting;

namespace BloodBank.Testing.IntegrationTests.Infrastructure.Fixtures;

public class SharedTestFixture : IAsyncLifetime
{
    public MsSqlContainer DatabaseContainer { get; private set; }
    public RabbitMqContainer MessageBusContainer { get; private set; }
    protected IHost HostTest { get; private set; }
    public IServiceProvider ServiceProvider { get; private set; }
    public IConfiguration Configuration { get; private set; }
    
    public async Task InitializeAsync()
    {    
            DatabaseContainer = SqlServerTestContainer.CreateContainer();
            MessageBusContainer = RabbitMqTestContainer.CreateContainer();
            
            await Task.WhenAll(
                DatabaseContainer.StartAsync(),
                MessageBusContainer.StartAsync()
            );

            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(GetConfigSettings()!)
                .Build();;
    }

    public async Task SetupHostAndInfra(Action<IServiceCollection>? replaceActions = null)
    {
        if (HostTest != null)
        {
            await HostTest.StopAsync();
            await Task.Delay(1000); 
            HostTest.Dispose();
            HostTest = null;
        }
        
        HostTest = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton(Configuration);
                services.AddInfrastructure(Configuration).GetAwaiter().GetResult();

                if (replaceActions != null) replaceActions(services);
            })
            .Build();
        
        ServiceProvider = HostTest.Services;
        
        using var scope = HostTest.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();
        db.Database.EnsureCreated();
        
        await HostTest.StartAsync();
    }
    public Dictionary<string, string> GetConfigSettings()
    {
        return new Dictionary<string, string>()
        {
            {"ConnectionStrings:BloodBankCs", DatabaseContainer.GetConnectionString()},
            {"MessageBusSettings:HostName", "localhost"},
            {"MessageBusSettings:Port", MessageBusContainer.GetMappedPublicPort(5672).ToString()},
            {"MessageBusSettings:UserName", "guest"},
            {"MessageBusSettings:Password", "guest"},
            {"Outbox:BatchSize", "10"},
            {"Outbox:ImmediateRetryCount", "3"},
            {"Outbox:MessageRetryLimit", "5"},
            {"Outbox:ProcessingIntervalSeconds", "10"}, 
            {"Outbox:MessageLockTimeoutSeconds", "15"},
            {"Outbox:CircuitBreakerThreshold", "10"},
            {"Outbox:CircuitBreakerResetSeconds", "5"},
            
            {"MessageBusSettings:Exchanges:bloodbank.donations:Type", "topic"},
            {"MessageBusSettings:Exchanges:bloodbank.donations:Durable", "true"},
            {"MessageBusSettings:Exchanges:bloodbank.donations:AutoDelete", "false"},
            
            {"MessageBusSettings:Exchanges:bloodbank.dlx:Type", "topic"},
            {"MessageBusSettings:Exchanges:bloodbank.dlx:Durable", "true"},
            {"MessageBusSettings:Exchanges:bloodbank.dlx:AutoDelete", "false"},
            
            {"MessageBusSettings:Queues:donations.registration.process:Exchange", "bloodbank.donations"},
            {"MessageBusSettings:Queues:donations.registration.process:RoutingKey", "donation-registered"},
            {"MessageBusSettings:Queues:donations.registration.process:Durable", "true"},
            {"MessageBusSettings:Queues:donations.registration.process:Exclusive", "false"},
            {"MessageBusSettings:Queues:donations.registration.process:AutoDelete", "false"},
            {"MessageBusSettings:Queues:donations.registration.process:Arguments:x-dead-letter-exchange", "bloodbank.dlx"},
            {"MessageBusSettings:Queues:donations.registration.process:Arguments:x-dead-letter-routing-key", "deadletter.donations.process"},
            
            {"MessageBusSettings:Queues:donations.registration.email:Exchange", "bloodbank.donations"},
            {"MessageBusSettings:Queues:donations.registration.email:RoutingKey", "donation-registered"},
            {"MessageBusSettings:Queues:donations.registration.email:Durable", "true"},
            {"MessageBusSettings:Queues:donations.registration.email:Exclusive", "false"},
            {"MessageBusSettings:Queues:donations.registration.email:AutoDelete", "false"},
            {"MessageBusSettings:Queues:donations.registration.email:Arguments:x-dead-letter-exchange", "bloodbank.dlx"},
            {"MessageBusSettings:Queues:donations.registration.email:Arguments:x-dead-letter-routing-key", "deadletter.donations.email"},
            
            {"MessageBusSettings:Queues:bloodbank.deadletter:Exchange", "bloodbank.dlx"},
            {"MessageBusSettings:Queues:bloodbank.deadletter:RoutingKey", "#"},
            {"MessageBusSettings:Queues:bloodbank.deadletter:Durable", "true"},
            {"MessageBusSettings:Queues:bloodbank.deadletter:Exclusive", "false"},
            {"MessageBusSettings:Queues:bloodbank.deadletter:AutoDelete", "false"},
            
            {"Brevo:ApiKey", "FakeApiKey"},
            {"Brevo:FromEmail", "fakeemail@email.com"},
            {"Brevo:FromName", "BloodBank"}
        };
    }

    public async Task DisposeAsync()
    {
        if (HostTest != null)
        {
            await HostTest.StopAsync();
            HostTest.Dispose();
        }

        if (DatabaseContainer != null)
        {
            await DatabaseContainer.StopAsync();
            await DatabaseContainer.DisposeAsync();
        }
        
        if (MessageBusContainer != null)
        {
            await MessageBusContainer.StopAsync(); 
            await MessageBusContainer.DisposeAsync();
        }
    }
} 