using BloodBank.Application.ViewModels;
using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using MediatR;

namespace BloodBank.Application.Queries.Donors.GetAllDonors;

public class GetAllDonorsHandler : IRequestHandler<GetAllDonorsQuery, ResultViewModel<List<BloodDonorViewModel>>>
{
    private readonly IBloodDonorsRepository _repository;

    public GetAllDonorsHandler(IBloodDonorsRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultViewModel<List<BloodDonorViewModel>>> Handle(GetAllDonorsQuery request, CancellationToken cancellationToken)
    {
        var donors = await _repository.GetAllAsync();
        var model = donors.Select(BloodDonorViewModel.fromEntity).ToList();
        return ResultViewModel<List<BloodDonorViewModel>>.Success(model);
        
    }
}