using BloodBank.Application.ViewModels;
using MediatR;

namespace BloodBank.Application.Queries.Donors.GetDonorById;

public class GetDonorByIdQuery : IRequest<ResultViewModel<BloodDonorViewModel>>
{
    public GetDonorByIdQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}