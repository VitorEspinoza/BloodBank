using BloodBank.Application.ViewModels;
using BloodBank.Core.Entities;
using MediatR;

namespace BloodBank.Application.Commands.Donations.RegisterDonation;

public class RegisterDonationCommand : IRequest<ResultViewModel<int>>
{
    public int BloodDonorId { get; set; }
    public int QuantityInMl { get; set; }
    public Donation ToEntity(BloodDonor bloodDonor) => new(bloodDonor,  QuantityInMl);
}