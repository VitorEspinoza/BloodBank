using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;
using BloodBank.Infrastructure.Services.Reports.Models;

namespace BloodBank.Infrastructure.Reports;

public class BloodQuantityByTypeReport : IDocument
{
    public BloodQuantityByTypeReport(List<BloodQuantityByTypeReportData> reportData)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.EnableDebugging = true; 
        ReportData = reportData;
    }

    public List<BloodQuantityByTypeReportData> ReportData { get; set; }
    
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11));

            page.Header()
                .PaddingBottom(10)
                .AlignCenter()
                .Text("Blood Stock Report - Quantity by Blood Type")
                .Bold()
                .FontSize(16)
                .FontColor(Colors.Blue.Darken3);

            page.Content()
                .PaddingVertical(10)
                .Column(column =>
                {
                    column.Spacing(10);

                    if (ReportData.Any())
                    {
                        // Principal Table
                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // Blood Type
                                    columns.RelativeColumn(3); // Quantity (ml)
                                    columns.RelativeColumn(2); // Percentage
                                });
                                
                                table.Header(header =>
                                {
                                    header.Cell().Text("Blood Type").Bold();
                                    header.Cell().AlignRight().Text("Quantity (ml)").Bold();
                                    header.Cell().AlignRight().Text("Percentage").Bold();

                                    header.Cell().ColumnSpan(3)
                                        .PaddingTop(5)
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Black);
                                });

                                var totalQuantity = ReportData.Sum(x => x.QuantityMl);
                                
                                foreach (var item in ReportData.OrderByDescending(x => x.QuantityMl))
                                {
                                    var percentage = totalQuantity > 0 ? 
                                        (item.QuantityMl * 100.0 / totalQuantity) : 0;
                                    
                                    var bloodTypeColor = item.BloodType.EndsWith("-") ? 
                                        Colors.Red.Darken2 : Colors.Black;
                                    
                                    table.Cell()
                                        .PaddingVertical(5)
                                        .Text(item.BloodType)
                                        .FontColor(bloodTypeColor);
                                    
                                    table.Cell()
                                        .PaddingVertical(5)
                                        .AlignRight()
                                        .Text(item.QuantityMl.ToString("N0"));
                                    
                                    table.Cell()
                                        .PaddingVertical(5)
                                        .AlignRight()
                                        .Text($"{percentage:0.00}%");
                                }

                                // Total line
                                table.Cell()
                                    .PaddingVertical(5)
                                    .Text("TOTAL")
                                    .Bold();
                                
                                table.Cell()
                                    .PaddingVertical(5)
                                    .AlignRight()
                                    .Text(totalQuantity.ToString("N0"))
                                    .Bold();
                                
                                table.Cell()
                                    .PaddingVertical(5)
                                    .AlignRight()
                                    .Text("100%")
                                    .Bold();
                            });

                        // SVG Graphic
                        column.Item()
                            .PaddingTop(20)
                            .Svg(GenerateBarChartSvg());
                    }
                    else
                    {
                        column.Item()
                            .PaddingTop(50)
                            .AlignCenter()
                            .Text("No blood stock data available")
                            .FontColor(Colors.Grey.Medium);
                    }
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

    private string GenerateBarChartSvg()
    {
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        try
        {
            if (ReportData.Count == 0)
                return string.Empty;

            var totalQuantity = ReportData.Sum(x => x.QuantityMl);
            if (totalQuantity == 0)
                return string.Empty;

            var maxValue = ReportData.Max(x => x.QuantityMl);
            var orderedData = ReportData.OrderByDescending(x => x.QuantityMl).ToList();

            var svgWidth = 650;
            var svgHeight = 200;
            var barWidth = 50;
            var spacing = 20;
            var chartHeight = 150;
            var startX = 50;
            var startY = svgHeight - 30;

            var svg = new StringBuilder();
            svg.AppendLine($@"<svg width='{svgWidth}' height='{svgHeight}' xmlns='http://www.w3.org/2000/svg'>");

            // Axes
            svg.AppendLine(
                $@"<line x1='{startX}' y1='20' x2='{startX}' y2='{startY}' stroke='black' stroke-width='1' />");
            svg.AppendLine(
                $@"<line x1='{startX}' y1='{startY}' x2='{svgWidth - 20}' y2='{startY}' stroke='black' stroke-width='1' />");

            // Bars
            for (int i = 0; i < orderedData.Count; i++)
            {
                var item = orderedData[i];
                var barHeight = (item.QuantityMl / (float)maxValue) * chartHeight;
                var x = startX + (i * (barWidth + spacing)) + 30;
                var y = startY - barHeight;

                var color = item.BloodType.EndsWith("-") ? "red" : "blue";

                // Bar
                svg.AppendLine($@"<rect x='{x}' y='{y}' width='{barWidth}' height='{barHeight}' fill='{color}' />");

                svg.AppendLine(
                    $@"<text x='{x + barWidth / 2}' y='{y - 5}' font-size='10' text-anchor='middle'>{item.QuantityMl}</text>");

                // Blood Type
                svg.AppendLine(
                    $@"<text x='{x + barWidth / 2}' y='{startY + 15}' font-size='10' text-anchor='middle'>{item.BloodType}</text>");
            }

            svg.AppendLine("</svg>");
            return svg.ToString();
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }
        
       
}