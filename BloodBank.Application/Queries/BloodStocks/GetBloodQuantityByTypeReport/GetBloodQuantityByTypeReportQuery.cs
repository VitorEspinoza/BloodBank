using BloodBank.Infrastructure.Services.Reports.Models;
using MediatR;

namespace BloodBank.Application.Queries.BloodStocks.GetBloodQuantityByTypeReport
{
    public class GetBloodQuantityByTypeReportQuery : IRequest<List<BloodQuantityByTypeReportData>>
    {
    
    }
}