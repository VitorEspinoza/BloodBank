using BloodBank.Core.ValueObjects;
using BloodBank.Infrastructure.Services.Reports.Models;

namespace BloodBank.Infrastructure.Services.Reports;

public interface IReportsService
{
    public Task<List<BloodQuantityByTypeReportData>> GetBloodQuantityByTypeAsync(CancellationToken cancellationToken);
    public Task<List<DonationReportData>> GetRecentDonationsReportDataAsync(CancellationToken cancellationToken);
}