using BloodBank.Core.DomainEvents.Donations;
using BloodBank.Infrastructure.Services.Notification;
using BloodBank.Infrastructure.Services.Notification.Brevo;
using BloodBank.Infrastructure.Services.Notification.Interfaces;
using BloodBank.Testing.Common.Fakers;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Infrastructure.Services.Notification;

public class DonationEmailServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService<BrevoEmailRequest> _emailService;
    private readonly DonationEmailService _service;
    private readonly DonationRegistered _testEvent;

    public DonationEmailServiceTests()
    {
        _configuration = Substitute.For<IConfiguration>();
        _emailService = Substitute.For<IEmailService<BrevoEmailRequest>>();
        _service = new DonationEmailService(_configuration, _emailService);
        _testEvent = DonationRegisteredFaker.Generate();
    }

    [Fact]
    public async Task SendThankYouEmailAsync_ShouldBuildCorrectEmail()
    {
        await _service.SendThankYouEmailAsync(_testEvent);
        
        await _emailService.Received(1).SendAsync(Arg.Is<BrevoEmailRequest>(req =>
            req.To.First().Email == _testEvent.BloodDonorEmail &&
            req.Subject.Contains(_testEvent.DonationId.ToString()) &&
            req.HtmlContent.Contains(_testEvent.BloodDonorName) &&
            req.HtmlContent.Contains(_testEvent.QuantityMl.ToString()) &&
            req.Attachment.Count == 1
        ));
    }

    [Fact]
    public async Task SendThankYouEmailAsync_ShouldIncludeCertificateAttachment()
    {
        await _service.SendThankYouEmailAsync(_testEvent);
        
        await _emailService.Received(1).SendAsync(Arg.Is<BrevoEmailRequest>(req =>
            req.Attachment[0].Name.Contains(_testEvent.DonationId.ToString())
        ));
    }

    [Fact]
    public async Task SendThankYouEmailAsync_ShouldHandleEmailServiceErrors()
    {
        _emailService.When(x => x.SendAsync(Arg.Any<BrevoEmailRequest>()))
                   .Throw(new Exception("Email service error"));
        
        await Assert.ThrowsAsync<Exception>(() => 
            _service.SendThankYouEmailAsync(_testEvent));
    }
    
}