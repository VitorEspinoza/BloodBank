using System.Net;
using BloodBank.Infrastructure.Services.Notification.Brevo;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace BloodBank.Testing.UnitTests.Infrastructure.Services.Notification.Brevo;

public class BrevoEmailServiceTests
{
    private readonly BrevoEmailRequest _sampleRequest;
    private const string BaseUrl = "https://api.brevo.com/v3/";
 
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly BrevoEmailService _service;
    public BrevoEmailServiceTests()
    {
        _sampleRequest = new BrevoEmailRequest
        {
            Sender = new BrevoContact { Email = "sender@example.com", Name = "Sender Name" },
            To = new List<BrevoContact> { new() { Email = "recipient@example.com", Name = "Recipient Name" } },
            Subject = "Test Subject",
            HtmlContent = "<p>Test content</p>"
        };
        _mockHttp  = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(BaseUrl);
        
        _service = new BrevoEmailService(httpClient);
    }

    [Fact]
    public async Task ValidEmailRequest_SendAsync_ShouldSendRequestToCorrectEndpoint()
    {
        
        _mockHttp.Expect(HttpMethod.Post, $"{BaseUrl}smtp/email")
                .Respond(HttpStatusCode.OK);

        await _service.SendAsync(_sampleRequest);

        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task ValidEmailRequest_SendAsync_ShouldSerializeRequestWithCamelCase()
    {
        _mockHttp.Expect(HttpMethod.Post, $"{BaseUrl}smtp/email")
                .WithPartialContent("\"sender\"")
                .WithPartialContent("\"to\"")
                .WithPartialContent("\"subject\"")
                .WithPartialContent("\"htmlContent\"")
                .Respond(HttpStatusCode.OK);

        await _service.SendAsync(_sampleRequest);

        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task ValidEmailRequest_SendAsync_ShouldUseCorrectContentType()
    {
        
        _mockHttp.Expect(HttpMethod.Post, $"{BaseUrl}smtp/email")
                .WithHeaders("Content-Type", "application/json; charset=utf-8")
                .Respond(HttpStatusCode.OK);

        await _service.SendAsync(_sampleRequest);

        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task SuccessfulResponse_SendAsync_ShouldNotThrowException()
    {
        _mockHttp.When(HttpMethod.Post, $"{BaseUrl}smtp/email")
                .Respond(HttpStatusCode.OK);

        await _service.SendAsync(_sampleRequest); 
    }

    [Fact]
    public async Task ErrorResponse_SendAsync_ShouldThrowHttpRequestException()
    {
        _mockHttp.When(HttpMethod.Post, $"{BaseUrl}smtp/email")
                .Respond(HttpStatusCode.BadRequest);
        
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _service.SendAsync(_sampleRequest));
    }

    [Fact]
    public async Task ServerErrorResponse_SendAsync_ShouldThrowHttpRequestException()
    {
        
        _mockHttp.When(HttpMethod.Post, $"{BaseUrl}smtp/email")
                .Respond(HttpStatusCode.InternalServerError);
        
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _service.SendAsync(_sampleRequest));
    }
}