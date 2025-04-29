
using System.Net.Http.Headers;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.BackgroundServices;
using BloodBank.Infrastructure.MessageBus;
using BloodBank.Infrastructure.MessageBus.Interfaces;
using BloodBank.Infrastructure.MessageBus.TopologyConfig;
using BloodBank.Infrastructure.MessageBus.TopologyConfig.ConfigsDefinition;
using BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Infrastructure.Persistence.Repositories;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using BloodBank.Infrastructure.Services.Address.ViaCep;
using BloodBank.Infrastructure.Services.Notification;
using BloodBank.Infrastructure.Services.Notification.Brevo;
using BloodBank.Infrastructure.Services.Notification.Interfaces;
using BloodBank.Infrastructure.Services.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace BloodBank.Infrastructure;

public static class InfrastructureModule
{
    public static async Task<IServiceCollection> AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        await services
            .AddMessageBus(configuration);

        services
            .AddRepositories()
            .AddServices(configuration)
            .AddData(configuration)
            .AddUnitOfWork()
            .AddBackgroundServices();

        return services;
    }

    private static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        
        var connectionString = configuration.GetConnectionString("BloodBankCs");

        services.AddDbContext<BloodBankDbContext>(o => o.UseSqlServer(connectionString));
        
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBloodStockRepository, BloodStockRepository>();
        services.AddScoped<IBloodDonorsRepository, BloodDonorsRepository>();
        services.AddScoped<IDonationRepository, DonationRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IProcessedEventRespository, ProcessedEventRepository>();
        
        return services; 
    }
    
    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
    private static IServiceCollection AddServices(this IServiceCollection services,  IConfiguration configuration)
    {
        services.AddHttpClient<IAddressService, ViaCepAddressService>( client =>
        {
            client.BaseAddress = new Uri("https://viacep.com.br/ws/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        
        services.AddHttpClient<IEmailService<BrevoEmailRequest>, BrevoEmailService>(client =>
        {
            client.BaseAddress = new Uri("https://api.brevo.com/v3/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("api-key", configuration["Brevo:ApiKey"]);
        });
        
        services.AddScoped<IDonationEmailService, DonationEmailService>();
        services.AddScoped<IReportsService, ReportsService>();
        
        return services;
    }

    private static async Task<IServiceCollection> AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
  
        services.Configure<MessageBusSettings>(
            configuration.GetSection("MessageBusSettings"));
        services.Configure<OutboxSettings>(
            configuration.GetSection("Outbox"));
        
        services.AddSingleton<IEventBusTopologyDefinition, TopologyDefinition>();
        services.AddSingleton<ITopologyContext, TopologyContext>();
        
        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
        };
        var connection = await connectionFactory.CreateConnectionAsync("bloodbank-message-bus");
        services.AddSingleton(new ProducerConnection(connection));
        
        services.AddSingleton<RabbitMqChannelPool>(sp => 
            new RabbitMqChannelPool(sp.GetRequiredService<ProducerConnection>().Connection));
        
        services.AddSingleton<IEventExchangeResolver, DefaultExchangeResolver>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        var pool = serviceProvider.GetRequiredService<RabbitMqChannelPool>();
        var topologyContext = serviceProvider.GetRequiredService<ITopologyContext>();
        var topologyDefinition = serviceProvider.GetRequiredService<IEventBusTopologyDefinition>();
        
        var messageBusClient = await RabbitMqClientFactory.CreateClientAsync(
            topologyContext,
            topologyDefinition,
            pool
        );
        
        services.AddSingleton(messageBusClient);


        return services;
    }
    
    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OrphanAddressCleanupJob>();
        services.AddHostedService<OutboxProcessorJob>();
        services.AddHostedService<OutboxCleanupJob>();
        
        return services;
    }
    
}