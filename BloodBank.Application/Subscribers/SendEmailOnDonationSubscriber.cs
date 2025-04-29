using BloodBank.Core.DomainEvents.Donations;
using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;
using BloodBank.Infrastructure.Services.Notification.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BloodBank.Application.Subscribers;

public class SendEmailOnDonationSubscriber : BaseSubscriber<DonationRegistered>
{
    
    public SendEmailOnDonationSubscriber(IServiceProvider serviceProvider) 
        : base( serviceProvider,
            "donations.registration.email"
           )
    {
    }

    protected override async Task<bool> ProcessEventAsync(DonationRegistered @event, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var eventId = @event.DonationId;
        var consumerName = GetType().Name;
        var processEventRepository = serviceProvider.GetRequiredService<IProcessedEventRespository>();
    
        if(await processEventRepository.ExistsAsync(eventId, consumerName, cancellationToken))
            return true;

        try
        {
            var donationEmailService = serviceProvider.GetRequiredService<IDonationEmailService>();
            await donationEmailService.SendThankYouEmailAsync(@event);
        
            var processedEvent = new ProcessedEvent(eventId, consumerName, nameof(DonationRegistered));
            await processEventRepository.MarkAsSuccessfulAsync(processedEvent, cancellationToken, true);
        
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
}