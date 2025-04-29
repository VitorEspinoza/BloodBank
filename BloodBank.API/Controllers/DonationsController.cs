using BloodBank.Application.Commands.Donations.RegisterDonation;
using BloodBank.Application.Queries.Donations.GetAllDonations;
using BloodBank.Application.Queries.Donations.GetDonationById;
using BloodBank.Application.Queries.Donations.GetRecentDonationsReport;
using BloodBank.Infrastructure.Reports;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;


namespace BloodBank.API.Controllers
{
    [Route("api/donations")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DonationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
           var donations = await _mediator.Send(new GetAllDonationsQuery());
           
            return Ok(donations);
        }
        
        [HttpGet("report/recent")]
        public async Task<IResult> GetRecentDonationsReport()
        {
            var donations = await _mediator.Send(new GetRecentDonationsReportQuery());

            var report = new RecentDonationsReport(donations);
            var pdf = report.GeneratePdf();
            
            return Results.File(pdf, "application/pdf");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var donation = await _mediator.Send(new GetDonationByIdQuery(id));
            
            if (donation == null) return NotFound();
            
            return Ok(donation);
        }

        [HttpPost]
        public async Task<IActionResult> Post(RegisterDonationCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Created();
        }
        
    }
}
