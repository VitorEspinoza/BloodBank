using BloodBank.Core.DomainEvents.Donations;

namespace BloodBank.Infrastructure.Services.Notification.Interfaces;

public interface IDonationEmailService
{
    Task SendThankYouEmailAsync(DonationRegistered donationRegisteredEvent);
}