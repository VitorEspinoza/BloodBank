namespace BloodBank.Infrastructure.Services.Notification.Interfaces;

public interface IEmailService<TEmailFormat>
{
    Task SendAsync(TEmailFormat request);
}