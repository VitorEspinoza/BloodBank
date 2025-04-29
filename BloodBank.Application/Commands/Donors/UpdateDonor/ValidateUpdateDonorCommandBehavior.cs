using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using MediatR;

namespace BloodBank.Application.Commands.Donors.UpdateDonor;

public class ValidateUpdateDonorCommandBehavior : IPipelineBehavior<UpdateDonorCommand, ResultViewModel>
{
    private readonly IBloodDonorsRepository _repository;
    private readonly IAddressService _addressService;

    public ValidateUpdateDonorCommandBehavior(IBloodDonorsRepository repository, IAddressService addressService)
    {
        _repository = repository;
        _addressService = addressService;
    }

    public async Task<ResultViewModel> Handle(UpdateDonorCommand request, RequestHandlerDelegate<ResultViewModel> next, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        
        var bloodDonor = await _repository.GetById(request.Id);
        if (bloodDonor is null)
        {
            errors.Add($"Donor with ID {request.Id} not found");
            return ResultViewModel<int>.Error(errors);
        }
        
        var emailIsChanged = bloodDonor.Email != request.Email;
        var emailAlreadyRegistered = emailIsChanged && await _repository.Exists(request.Email);

        if (emailAlreadyRegistered)
        {
            errors.Add($"Email {request.Email} already registered");
            return ResultViewModel<int>.Error(errors);
        }
            
        var addressChanged = bloodDonor.Address.ZipCode != request.Zipcode || bloodDonor.Address.Complement != request.Complement || bloodDonor.Address.Number != request.Number;
        if (!addressChanged) return await next();
        
        var addressValidationResult = await _addressService.ValidateAddressAsync(request.Zipcode);
        
        if (addressValidationResult.IsValid) return await next();
        
        errors.Add($"Invalid Address");
        return ResultViewModel<int>.Error(errors);
    }
}