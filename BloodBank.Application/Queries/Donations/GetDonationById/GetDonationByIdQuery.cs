using BloodBank.Application.ViewModels;
using MediatR;

namespace BloodBank.Application.Queries.Donations.GetDonationById;

public class GetDonationByIdQuery : IRequest<ResultViewModel<DonationViewModel>>
{
    public GetDonationByIdQuery(int id)
    {
        Id = id;
    }
    public int Id { get; set; }
}