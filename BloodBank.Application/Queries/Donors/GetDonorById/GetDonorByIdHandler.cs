using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using MediatR;

namespace BloodBank.Application.Queries.Donors.GetDonorById;

public class GetDonorByIdHandler : IRequestHandler<GetDonorByIdQuery,  ResultViewModel<BloodDonorViewModel>?>
{
    private readonly IBloodDonorsRepository _repository;

    public GetDonorByIdHandler(IBloodDonorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultViewModel<BloodDonorViewModel>?> Handle(GetDonorByIdQuery request, CancellationToken cancellationToken)
    {
        var bloodDonor = await _repository.GetById(request.Id);

        if (bloodDonor == null) return null;

        var model = BloodDonorViewModel.fromEntity(bloodDonor);
        return ResultViewModel<BloodDonorViewModel>.Success(model);
    }
}