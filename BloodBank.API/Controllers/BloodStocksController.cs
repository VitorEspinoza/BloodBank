using BloodBank.Application.Queries.BloodStocks.GetBloodQuantityByTypeReport;
using BloodBank.Core.DomainEvents.Donations;
using BloodBank.Core.Enums;
using BloodBank.Core.ValueObjects;
using BloodBank.Infrastructure.Reports;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace BloodBank.API.Controllers
{
    [Route("api/blood-stocks")]
    [ApiController]
    public class BloodStocksController : ControllerBase
    {
        private readonly  IMediator _mediator;

        public BloodStocksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("report/blood-by-type")]
        public async Task<IResult> GetRecentDonationsReport()
        {
            var reportData = await _mediator.Send(new GetBloodQuantityByTypeReportQuery());

            var report = new BloodQuantityByTypeReport(reportData);
            var pdf = report.GeneratePdf();
            
            return Results.File(pdf, "application/pdf");
        }
    }
}
