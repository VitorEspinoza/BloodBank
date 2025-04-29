using BloodBank.Infrastructure.Services.Reports.Models;
using MediatR;

namespace BloodBank.Application.Queries.Donations.GetRecentDonationsReport;

public class GetRecentDonationsReportQuery : IRequest<List<DonationReportData>>
{
    
}