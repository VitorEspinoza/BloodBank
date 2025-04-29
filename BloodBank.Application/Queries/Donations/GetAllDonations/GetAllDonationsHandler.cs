using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using MediatR;

namespace BloodBank.Application.Queries.Donations.GetAllDonations;

public class GetAllDonationsHandler : IRequestHandler<GetAllDonationsQuery, ResultViewModel<List<DonationViewModel>>>
{
    private readonly IDonationRepository _repository;

    public GetAllDonationsHandler(IDonationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultViewModel<List<DonationViewModel>>> Handle(GetAllDonationsQuery request, CancellationToken cancellationToken)
    {
        var donations = await _repository.GetAll();
        
        var model = donations.Select(DonationViewModel.fromEntity).ToList();
        
        return ResultViewModel<List<DonationViewModel>>.Success(model);
    }
}