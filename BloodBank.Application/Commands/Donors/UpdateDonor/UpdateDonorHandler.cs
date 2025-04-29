using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using MediatR;

namespace BloodBank.Application.Commands.Donors.UpdateDonor;

public class UpdateDonorHandler : IRequestHandler<UpdateDonorCommand, ResultViewModel>
{
    private readonly IAddressService _addressService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBloodDonorsRepository  _bloodDonorsRepository;
    public UpdateDonorHandler(IBloodDonorsRepository repository, IAddressService addressService, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _bloodDonorsRepository = repository;
        _addressService = addressService;
    }

    public async Task<ResultViewModel> Handle(UpdateDonorCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var bloodDonor = await _bloodDonorsRepository.GetById(request.Id);
     
            var addressChanged = bloodDonor!.Address.ZipCode != request.Zipcode || bloodDonor.Address.Number != request.Number || bloodDonor.Address.Complement != request.Complement;
            if (addressChanged)
            {
                var address = await _addressService.PersistAddressAsync(request.Zipcode, request.Number, request.Complement);
                bloodDonor.UpdateAddress(address);
            }

            bloodDonor.Update(
                request.FullName,
                request.Email,
                bloodDonor.BirthDate, 
                bloodDonor.BiologicalSex,
                request.Weight,
                bloodDonor.BloodType
            );
        
            _bloodDonorsRepository.Update(bloodDonor);
        
            return ResultViewModel.Success();
        }, cancellationToken);
        
      
    }
    
}