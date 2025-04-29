using BloodBank.Core.DomainEvents.Donations;
using BloodBank.Infrastructure.ExternalServices.Notification.Brevo;
using BloodBank.Infrastructure.Reports;
using BloodBank.Infrastructure.Services.Notification.Brevo;
using BloodBank.Infrastructure.Services.Notification.Interfaces;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;

namespace BloodBank.Infrastructure.Services.Notification;

public class DonationEmailService : IDonationEmailService
{
   private readonly IConfiguration _configuration;
   private readonly IEmailService<BrevoEmailRequest> _emailService;

   public DonationEmailService(IConfiguration configuration, IEmailService<BrevoEmailRequest> emailService)
   {
       _configuration = configuration;
       _emailService = emailService;
   }

   public async Task SendThankYouEmailAsync(DonationRegistered donationRegisteredEvent)
    {
        var brevoEmailBuilder = new BrevoEmailBuilder(_configuration);
        
        var certificatePdf = new BloodDonationCertificate(donationRegisteredEvent).GeneratePdf();
        var htmlContent = BuildEmailHtml(donationRegisteredEvent);
        
        var email = brevoEmailBuilder
            .To(donationRegisteredEvent.BloodDonorEmail)
            .WithSubject($"Thank You for Your Blood Donation - Certificate #{donationRegisteredEvent.DonationId}")
            .WithHtmlContent(htmlContent)
            .Attach(
                certificatePdf,
                $"Donation_Certificate_{donationRegisteredEvent.DonationId}.pdf"
            )
            .Build();
        
        await _emailService.SendAsync(email);
    }
    
    
    private static string BuildEmailHtml(DonationRegistered donation)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset=""UTF-8"">
            <title>Thank You for Your Blood Donation</title>
            <style type=""text/css"">
                body {{
                    font-family: Arial, sans-serif;
                    line-height: 1.6;
                    color: #333333;
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                }}
                .header {{
                    color: #cc0000;
                    font-size: 24px;
                    margin-bottom: 20px;
                }}
                .footer {{
                    margin-top: 30px;
                    font-style: italic;
                    color: #666666;
                }}
            </style>
        </head>
        <body>
            <table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""20"">
                <tr>
                    <td>
                        <div class=""header"">Thank You for Your Blood Donation</div>
                        <p>Dear {donation.BloodDonorName},</p>
                        <p>Thank you for your generous blood donation of {donation.QuantityMl} ml 
                        on {donation.TriggeredAt:MMMM dd, yyyy}.</p>
                        <p>Your donation helps save lives. Please find attached your official 
                        donation certificate.</p>
                        <div class=""footer"">
                            <p>Best regards,<br>Blood Bank Team</p>
                        </div>
                    </td>
                </tr>
            </table>
        </body>
        </html>";
    }
}