using BloodBank.Infrastructure.Services.Reports;
using BloodBank.Infrastructure.Services.Reports.Models;
using MediatR;

namespace BloodBank.Application.Queries.BloodStocks.GetBloodQuantityByTypeReport
{
    public class GetBloodQuantityByTypeReportHandler : IRequestHandler<GetBloodQuantityByTypeReportQuery, List<BloodQuantityByTypeReportData>>
    {

        private readonly IReportsService _reportsService;
        public GetBloodQuantityByTypeReportHandler(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        public async Task<List<BloodQuantityByTypeReportData>> Handle(GetBloodQuantityByTypeReportQuery request, CancellationToken cancellationToken)
        {
            return await _reportsService.GetBloodQuantityByTypeAsync(cancellationToken);
        }
    }
}