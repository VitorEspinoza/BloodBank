using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using MediatR;

namespace BloodBank.Application.Queries.Donations.GetDonationById;

public class GetDonationByIdHandler : IRequestHandler<GetDonationByIdQuery, ResultViewModel<DonationViewModel>?>
{
    private readonly IDonationRepository _repository;

    public GetDonationByIdHandler(IDonationRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ResultViewModel<DonationViewModel>?> Handle(GetDonationByIdQuery request, CancellationToken cancellationToken)
    {
        var donation = await _repository.GetById(request.Id);

        if (donation == null) return null;
        
        var model = DonationViewModel.fromEntity(donation);
        
        return ResultViewModel<DonationViewModel>.Success(model);
    }
}