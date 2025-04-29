using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BloodBank.Core.DomainEvents.Donations;
using System;
using System.Globalization;

namespace BloodBank.Infrastructure.Reports;

public class BloodDonationCertificate : IDocument
{
    private readonly DonationRegistered _donationData;

    public BloodDonationCertificate(DonationRegistered donationData)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.EnableDebugging = true;
        _donationData = donationData;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11));

            page.Header()
                .PaddingBottom(20)
                .AlignCenter()
                .Column(column =>
                {
                    column.Item().Text("CERTIFICATE OF BLOOD DONATION")
                        .Bold()
                        .FontSize(24)
                        .FontColor(Colors.Red.Darken2);
                        
                    column.Item().PaddingTop(5).Text("Life Saving Contribution")
                        .FontSize(14)
                        .FontColor(Colors.Grey.Medium);

                    column.Item().PaddingTop(15).AlignCenter()
                        .Border(1)
                        .BorderColor(Colors.Red.Medium)
                        .Padding(10)
                        .Text("BLOOD BANK")
                        .Bold()
                        .FontSize(18)
                        .FontColor(Colors.Red.Darken1);
                });

            page.Content()
                .PaddingVertical(30)
                .Column(column =>
                {
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("This is to certify that").FontSize(14);
                    });
                    
                    column.Item().PaddingTop(20).AlignCenter().Text(_donationData.BloodDonorName)
                        .Bold()
                        .FontSize(22)
                        .FontColor(Colors.Red.Darken1);
                    
                    column.Item().PaddingTop(30).AlignCenter().Text(text =>
                    {
                        text.Span("Has generously donated").FontSize(14);
                    });
                    
                    column.Item().PaddingTop(10).AlignCenter().Text($"{_donationData.QuantityMl} ml")
                        .Bold()
                        .FontSize(20)
                        .FontColor(Colors.Red.Medium);
                        
                    column.Item().PaddingTop(5).AlignCenter().Text($"of {_donationData.BloodType} blood")
                        .Bold()
                        .FontSize(16);
                        
                    column.Item().PaddingTop(20).AlignCenter().Text($"on {_donationData.TriggeredAt.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)}")
                        .FontSize(14);
                        
                    column.Item().PaddingTop(40).AlignCenter().Text(text =>
                    {
                        text.Span("Your contribution helps save lives and is greatly appreciated.")
                            .FontSize(14)
                            .Italic();
                    });
                    
                    column.Item().PaddingTop(20).AlignCenter().Text($"Certificate ID: DON-{_donationData.DonationId:D6}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Medium);
                });

            page.Footer()
                .Column(column =>
                {
                    column.Item().PaddingTop(40).Row(row =>
                    {
                        row.RelativeItem(2);
                        
                        row.RelativeItem(4).Column(c =>
                        {
                            c.Item().PaddingBottom(5).BorderBottom(1).BorderColor(Colors.Black);
                            c.Item().AlignCenter().Text("Medical Director");
                        });
                        
                        row.RelativeItem(2);
                    });
                    
                    column.Item().AlignCenter().PaddingTop(30).Text(text =>
                    {
                        text.Span("Generated at: ").FontColor(Colors.Grey.Medium);
                        text.Span(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                    });
                });
        });
    }
}