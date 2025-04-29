using BloodBank.Application.ViewModels;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.ValueObjects;
using MediatR;

namespace BloodBank.Application.Commands.Donors.RegisterDonor;

public class RegisterDonorCommand : IRequest<ResultViewModel<int>>
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public string BiologicalSex { get; set; }
    public double Weight { get; set; }
    public string BloodTypeGroup { get; set; }
    public string RhFactor { get; set; }
    public string Zipcode { get; set;  }
    public string Number { get; set;  }
    public string? Complement { get; set;  }
    
    public BloodDonor ToEntity(Address address) => new (
        FullName,
        Email,
        BirthDate,
        Enum.Parse<BiologicalSex>(BiologicalSex, true),
        Weight,
        BloodType.FromDatabase(BloodTypeGroup, RhFactor),
        address
    );
}

