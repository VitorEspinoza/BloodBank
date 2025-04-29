using BloodBank.Application.Commands.Donors.RegisterDonor;
using BloodBank.Application.Commands.Donors.UpdateDonor;
using BloodBank.Application.Queries.Donors.GetAllDonors;
using BloodBank.Application.Queries.Donors.GetDonorById;
using BloodBank.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BloodBank.API.Controllers
{
    [Route("api/blood-donors")]
    [ApiController]
    public class BloodDonorsController : ControllerBase
    {

        private readonly IMediator _mediator;

        public BloodDonorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var donors = await _mediator.Send(new GetAllDonorsQuery());
            return Ok(donors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var donor = await _mediator.Send(new GetDonorByIdQuery(id));
            
            if (donor == null) return NotFound();

            return Ok(donor);
        }

        [HttpPost]
        public async Task<IActionResult> Post(RegisterDonorCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            
            return Created();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, UpdateDonorCommand command)
        {
            command.SetId(id);
            var result = await _mediator.Send(command);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            
            return NoContent();
        }
    }
}
