using BloodBank.Application.ViewModels;
using MediatR;

namespace BloodBank.Application.Queries.Donations.GetAllDonations;

public class GetAllDonationsQuery : IRequest<ResultViewModel<List<DonationViewModel>>>
{
    
}

