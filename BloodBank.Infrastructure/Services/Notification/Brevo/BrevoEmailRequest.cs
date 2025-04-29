namespace BloodBank.Infrastructure.Services.Notification.Brevo;

public class BrevoEmailRequest
{
    public BrevoContact Sender { get; set; }
    public List<BrevoContact> To { get; set; }
    public string Subject { get; set; }
    public string HtmlContent { get; set; }
    public List<BrevoAttachment> Attachment { get; set; }
}

public class BrevoContact
{
    public string Email { get; set; }
    public string Name { get; set; }
}

public class BrevoAttachment
{
    public string Content { get; set; } 
    public string Name { get; set; }
}