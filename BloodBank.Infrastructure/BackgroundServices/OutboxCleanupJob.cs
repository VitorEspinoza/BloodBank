using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BloodBank.Infrastructure.BackgroundServices;

public class OutboxCleanupJob : BackgroundService
{
    private readonly OutboxSettings _settings;
    private readonly IServiceProvider _serviceProvider;

    public OutboxCleanupJob(
        IServiceProvider serviceProvider,
        IOptions<OutboxSettings> settings)
    {
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        { 
            await CleanUpOldMessages(stoppingToken);
            
            try
            {
                await Task.Delay(TimeSpan.FromHours(_settings.CleanupIntervalHours), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
    

    private async Task CleanUpOldMessages(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var cutoffDate = DateTime.UtcNow.AddDays(-_settings.MessageRetentionDays);
        await outboxRepository.ArchiveProcessedMessagesAsync(cutoffDate, stoppingToken);
    }
}