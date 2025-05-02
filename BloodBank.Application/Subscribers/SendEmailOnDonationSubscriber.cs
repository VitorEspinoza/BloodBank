using BloodBank.Core.DomainEvents.Donations;
using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Services.Notification.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BloodBank.Application.Subscribers;

public class SendEmailOnDonationSubscriber : BaseSubscriber<DonationRegistered>
{
    private IServiceProvider _serviceProvider;
    public SendEmailOnDonationSubscriber(IServiceProvider serviceProvider) 
        : base(serviceProvider,
            "donations.registration.email"
           )
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<bool> ProcessEventAsync(DonationRegistered @event, CancellationToken cancellationToken)
    {
        var eventId = @event.DonationId;
        var consumerName = GetType().Name;
        
        using var scope = _serviceProvider.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
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