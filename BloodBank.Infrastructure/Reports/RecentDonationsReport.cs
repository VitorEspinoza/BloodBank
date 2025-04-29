using BloodBank.Infrastructure.Services.Reports;
using BloodBank.Infrastructure.Services.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BloodBank.Infrastructure.Reports;

public class RecentDonationsReport : IDocument
{
    public RecentDonationsReport(List<DonationReportData> donations)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        Donations = donations;
    }

    public List<DonationReportData> Donations { get; set; }
    
    public void Compose(IDocumentContainer container)
    {
       
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .AlignCenter()
                        .Text("Donations Report - Last 30 days")
                        .Bold()
                        .FontSize(16)
                        .FontColor(Colors.Blue.Darken3);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2); // Date
                                        columns.RelativeColumn(2); // Quantity
                                        columns.RelativeColumn(3); // Donor
                                        columns.RelativeColumn(1.5f); // Blood Type
                                        columns.RelativeColumn(2); // City
                                        columns.RelativeColumn(2); // State
                                    });
                                    
                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Date").Bold();
                                        header.Cell().Text("Quantity (ml)").Bold();
                                        header.Cell().Text("Donor").Bold();
                                        header.Cell().Text("Blood Type").Bold();
                                        header.Cell().Text("City").Bold();
                                        header.Cell().Text("State").Bold();

                                        header.Cell().ColumnSpan(6)
                                            .PaddingTop(5)
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Black);
                                    });

                                    foreach (var donation in Donations)
                                    {
                                        table.Cell().Text(donation.DonationDate.ToString("MM/dd/yyyy"));
                                        table.Cell().Text(donation.QuantityMl.ToString());
                                        table.Cell().Text(donation.DonorName);
                                        
                                        var bloodTypeColor = (donation.BloodType.EndsWith("-") && 
                                            (donation.BloodType.StartsWith("AB") || donation.BloodType.StartsWith("O")))
                                            ? Colors.Red.Darken2
                                            : Colors.Black;
                                        
                                        table.Cell().Text(donation.BloodType).FontColor(bloodTypeColor);
                                        table.Cell().Text(donation.DonorCity);
                                        table.Cell().Text(donation.DonorState);

                                        table.Cell().ColumnSpan(6)
                                            .PaddingTop(5)
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Grey.Lighten2);
                                    }
                                });

                            column.Item()
                                .PaddingTop(20)
                                .Grid(grid =>
                                {
                                    grid.Columns(3);
                                    
                                    grid.Item()
                                        .Background(Colors.Blue.Lighten5)
                                        .Padding(10)
                                        .Text($"Total donations: {Donations.Count()}")
                                        .Bold();
                                        
                                    grid.Item()
                                        .Background(Colors.Green.Lighten5)
                                        .Padding(10)
                                        .Text($"Total volume: {Donations.Sum(d => d.QuantityMl)} ml")
                                        .Bold();
                                        
                                    grid.Item()
                                        .Background(Colors.Orange.Lighten5)
                                        .Padding(10)
                                        .Text($"Blood types: {Donations.Select(d => d.BloodType).Distinct().Count()}")
                                        .Bold();
                                });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated at: ").FontColor(Colors.Grey.Medium);
                            text.Span(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                        });
                });
    }
}