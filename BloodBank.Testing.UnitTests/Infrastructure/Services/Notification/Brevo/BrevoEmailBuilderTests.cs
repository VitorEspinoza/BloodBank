using BloodBank.Infrastructure.ExternalServices.Notification.Brevo;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Infrastructure.Services.Notification.Brevo;

public class BrevoEmailBuilderTests
{
    private readonly IConfiguration _mockConfig;
    private const string FromEmail = "test@example.com";
    private const string FromName = "Test Sender";

    public BrevoEmailBuilderTests()
    {
        _mockConfig = Substitute.For<IConfiguration>();
        _mockConfig["Brevo:FromEmail"].Returns(FromEmail);
        _mockConfig["Brevo:FromName"].Returns(FromName);
    }

    [Fact]
    public void ValidConfig_ConstructorIsCalled_ShouldInitializeFromEmailAndName()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        var request = builder
            .To("recipient@example.com")
            .WithSubject("Test Subject")
            .Build();
        
        Assert.Equal(FromEmail, request.Sender.Email);
        Assert.Equal(FromName, request.Sender.Name);
    }

    [Fact]
    public void Recipient_ToIsCalled_ShouldAddRecipient()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        const string email = "recipient@example.com";
        const string name = "Recipient Name";

        builder.To(email, name);
        var request = builder.WithSubject("Test Subject").Build();

        Assert.Single(request.To);
        Assert.Equal(email, request.To[0].Email);
        Assert.Equal(name, request.To[0].Name);
    }

    [Fact]
    public void RecipientWithoutName_ToIsCalled_ShouldAddRecipientWithNullName()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        const string email = "recipient@example.com";

        builder.To(email);
        var request = builder.WithSubject("Test Subject").Build();

        Assert.Single(request.To);
        Assert.Equal(email, request.To[0].Email);
        Assert.Null(request.To[0].Name);
    }

    [Fact]
    public void Subject_WithSubjectIsCalled_ShouldSetSubject()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        const string subject = "Test Subject";

        builder.WithSubject(subject);
        var request = builder.To("recipient@example.com").Build();

        Assert.Equal(subject, request.Subject);
    }

    [Fact]
    public void HtmlContent_WithHtmlContentIsCalled_ShouldSetHtmlContent()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        const string htmlContent = "<p>Test content</p>";

        builder.WithHtmlContent(htmlContent);
        var request = builder
            .To("recipient@example.com")
            .WithSubject("Test Subject")
            .Build();

        Assert.Equal(htmlContent, request.HtmlContent);
    }

    [Fact]
    public void Attachment_AttachIsCalled_ShouldAddAttachment()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        var fileBytes = new byte[] { 1, 2, 3, 4 };
        const string fileName = "test.pdf";

        builder.Attach(fileBytes, fileName);
        var request = builder
            .To("recipient@example.com")
            .WithSubject("Test Subject")
            .Build();

        Assert.Single(request.Attachment);
        Assert.Equal(fileName, request.Attachment[0].Name);
        Assert.Equal(Convert.ToBase64String(fileBytes), request.Attachment[0].Content);
    }

    [Fact]
    public void MultipleRecipients_BuildIsCalled_ShouldAddAllRecipients()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        
        builder
            .To("recipient1@example.com", "Recipient 1")
            .To("recipient2@example.com", "Recipient 2")
            .WithSubject("Test Subject");
        var request = builder.Build();

        Assert.Equal(2, request.To.Count);
        Assert.Equal("recipient1@example.com", request.To[0].Email);
        Assert.Equal("Recipient 1", request.To[0].Name);
        Assert.Equal("recipient2@example.com", request.To[1].Email);
        Assert.Equal("Recipient 2", request.To[1].Name);
    }

    [Fact]
    public void MultipleAttachments_BuildIsCalled_ShouldAddAllAttachments()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        
        builder
            .To("recipient@example.com")
            .WithSubject("Test Subject")
            .Attach(new byte[] { 1, 2, 3 }, "file1.pdf")
            .Attach(new byte[] { 4, 5, 6 }, "file2.pdf");
        var request = builder.Build();

        Assert.Equal(2, request.Attachment.Count);
        Assert.Equal("file1.pdf", request.Attachment[0].Name);
        Assert.Equal("file2.pdf", request.Attachment[1].Name);
    }

    [Fact]
    public void NoAttachments_BuildIsCalled_ShouldSetAttachmentToNull()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        
        var request = builder
            .To("recipient@example.com")
            .WithSubject("Test Subject")
            .Build();

        Assert.Null(request.Attachment);
    }

    [Fact]
    public void NoSubject_BuildIsCalled_ShouldThrowArgumentException()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        builder.To("recipient@example.com");

        var exception = Assert.Throws<ArgumentException>(() => builder.Build());
        Assert.Equal("Subject is required", exception.Message);
    }

    [Fact]
    public void NoRecipients_BuildIsCalled_ShouldThrowArgumentException()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        builder.WithSubject("Test Subject");

        var exception = Assert.Throws<ArgumentException>(() => builder.Build());
        Assert.Equal("At least one recipient is required", exception.Message);
    }

    [Fact]
    public void FluentInterface_BuildIsCalled_ShouldReturnCorrectRequest()
    {
        var builder = new BrevoEmailBuilder(_mockConfig);
        const string htmlContent = "<p>Test content</p>";
        var fileBytes = new byte[] { 1, 2, 3, 4 };
        
        var request = builder
            .To("recipient@example.com", "Recipient Name")
            .WithSubject("Test Subject")
            .WithHtmlContent(htmlContent)
            .Attach(fileBytes, "test.pdf")
            .Build();

        Assert.Equal(FromEmail, request.Sender.Email);
        Assert.Equal(FromName, request.Sender.Name);
        Assert.Single(request.To);
        Assert.Equal("recipient@example.com", request.To[0].Email);
        Assert.Equal("Recipient Name", request.To[0].Name);
        Assert.Equal("Test Subject", request.Subject);
        Assert.Equal(htmlContent, request.HtmlContent);
        Assert.Single(request.Attachment);
        Assert.Equal("test.pdf", request.Attachment[0].Name);
        Assert.Equal(Convert.ToBase64String(fileBytes), request.Attachment[0].Content);
    }

}
