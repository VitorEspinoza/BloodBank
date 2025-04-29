using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using MediatR;

namespace BloodBank.Application.Commands.Donors.RegisterDonor;

public class RegisterDonorHandler : IRequestHandler<RegisterDonorCommand, ResultViewModel<int>>
{
    private readonly IAddressService  _addressService;
    private readonly IUnitOfWork  _unitOfWork;
    private readonly IBloodDonorsRepository  _bloodDonorsRepository;

    public RegisterDonorHandler(IAddressService addressService, IBloodDonorsRepository repository, IUnitOfWork unitOfWork)
    {
        _addressService = addressService;
        _unitOfWork = unitOfWork;
        _bloodDonorsRepository = repository;
    }

    public async Task<ResultViewModel<int>> Handle(RegisterDonorCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var address = await _addressService.PersistAddressAsync(request.Zipcode, request.Number, request.Complement);
            var bloodDonor = request.ToEntity(address);
            
            await _bloodDonorsRepository.AddAsync(bloodDonor);
            return ResultViewModel<int>.Success(bloodDonor.Id);
        }, cancellationToken);
    }
}