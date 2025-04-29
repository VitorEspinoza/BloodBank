using BloodBank.Application.ViewModels;
using MediatR;

namespace BloodBank.Application.Commands.Donors.UpdateDonor;

public class UpdateDonorCommand : IRequest<ResultViewModel>
{
    public int Id { get; private set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public double Weight { get; set; }
    public string Zipcode { get; set;  }
    public string Number { get; set;  }
    public string? Complement { get; set; }

    public void SetId(int id)
    {
        Id = id;
    }

 
}