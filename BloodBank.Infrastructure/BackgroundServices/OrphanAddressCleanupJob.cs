using BloodBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BloodBank.Infrastructure.BackgroundServices;

public class OrphanAddressCleanupJob : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(24);

    public OrphanAddressCleanupJob(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    private const int BatchSize = 1000;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_cleanupInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CleanUpOldOrphanAddresses(stoppingToken);
        }
    }
    
    private async Task CleanUpOldOrphanAddresses(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();
        do
        {
            var deleted = await context.Addresses
                .Where(a => !context.BloodDonors.Any(da => da.AddressId == a.Id))
                .Take(BatchSize)
                .ExecuteDeleteAsync(stoppingToken);

            if (deleted == 0) break;
        } while (true);
    }
}