using System.Text;
using System.Text.Json;
using BloodBank.Infrastructure.Services.Notification.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BloodBank.Infrastructure.Services.Notification.Brevo;

public class BrevoEmailService : IEmailService<BrevoEmailRequest>
{
    private readonly HttpClient _httpClient;
    private const string BrevoUri = "smtp/email";

    public BrevoEmailService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendAsync(BrevoEmailRequest request)
    {
        
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var response = await _httpClient.PostAsync(
            BrevoUri,
            new StringContent(JsonSerializer.Serialize(request, jsonOptions), 
                Encoding.UTF8, 
                "application/json"
            ));
        
        response.EnsureSuccessStatusCode();
    }
    
}