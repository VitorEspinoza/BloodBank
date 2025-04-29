using BloodBank.Infrastructure.Services.Reports;
using BloodBank.Infrastructure.Services.Reports.Models;
using MediatR;

namespace BloodBank.Application.Queries.Donations.GetRecentDonationsReport;

public class GetRecentDonationsReportHandler : IRequestHandler<GetRecentDonationsReportQuery, List<DonationReportData>>
{
  
    private readonly IReportsService _reportsService;
    public GetRecentDonationsReportHandler(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    public async Task<List<DonationReportData>> Handle(GetRecentDonationsReportQuery request, CancellationToken cancellationToken)
    {
        return await _reportsService.GetRecentDonationsReportDataAsync(cancellationToken);
    }
}