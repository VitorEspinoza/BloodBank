using BloodBank.Infrastructure.Services.Notification.Brevo;
using Microsoft.Extensions.Configuration;

namespace BloodBank.Infrastructure.ExternalServices.Notification.Brevo;

public class BrevoEmailBuilder
{
    private readonly string _fromEmail;
    private readonly string _fromName;
    
    private readonly List<BrevoContact> _to = [];
    private string _subject;
    private string _htmlContent;
    private readonly List<BrevoAttachment> _attachments = [];
    
    
    public BrevoEmailBuilder(IConfiguration config)
    {
        _fromEmail = config["Brevo:FromEmail"];
        _fromName = config["Brevo:FromName"];
    }

    public BrevoEmailBuilder To(string email, string name = null)
    {
        _to.Add(new BrevoContact{Email = email, Name = name});
        return this;
    }

    public BrevoEmailBuilder WithSubject(string subject)
    {
        _subject = subject;
        return this;
    }

    public BrevoEmailBuilder WithHtmlContent(string html)
    {
        _htmlContent = html;
        return this;
    }

    public BrevoEmailBuilder Attach(byte[] fileBytes, string fileName)
    {
        _attachments.Add(new BrevoAttachment
        { 
            Content = Convert.ToBase64String(fileBytes), 
            Name = fileName 
        });
        return this;
    }
    
    public BrevoEmailRequest Build()
    {
        if (string.IsNullOrEmpty(_subject))
            throw new ArgumentException("Subject is required");

        if (!_to.Any())
            throw new ArgumentException("At least one recipient is required");

        return new BrevoEmailRequest
        {
            Sender = new BrevoContact { Email = _fromEmail, Name = _fromName },
            To = _to,
            Subject = _subject,
            HtmlContent = _htmlContent,
            Attachment = _attachments.Count != 0 ? _attachments : null
        };
    }
    
}