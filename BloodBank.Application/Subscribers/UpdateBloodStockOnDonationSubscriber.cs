using BloodBank.Core.DomainEvents.Donations;
using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;


namespace BloodBank.Application.Subscribers;

public class UpdateBloodStockOnDonationSubscriber : BaseSubscriber<DonationRegistered>
{
    private IServiceProvider _serviceProvider;
    public UpdateBloodStockOnDonationSubscriber(IServiceProvider serviceProvider)
        : base(
            serviceProvider,
            queueName: "donations.registration.process")
    {
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task<bool> ProcessEventAsync(DonationRegistered @event, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        var eventId = @event.DonationId;
        var consumerName = GetType().Name;
        var bloodStockrepository = serviceProvider.GetRequiredService<IBloodStockRepository>();
        var processEventRepository = serviceProvider.GetRequiredService<IProcessedEventRespository>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        
        if(await processEventRepository.ExistsAsync(eventId, consumerName, cancellationToken))
            return true;
        
        return await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
                var stock = await bloodStockrepository.GetByTypeAsync(@event.BloodType);

                stock.AddQuantityInMl(@event.QuantityMl);
                await bloodStockrepository.UpdateAsync(stock);
                
                var processedEvent = new ProcessedEvent(eventId, consumerName, nameof(DonationRegistered));
                await processEventRepository.MarkAsSuccessfulAsync(processedEvent, cancellationToken);
                return true;
        }, cancellationToken);
        
     
    }
}