using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using MediatR;

namespace BloodBank.Application.Commands.Donors.RegisterDonor;

public class ValidateRegisterDonorCommandBehavior : IPipelineBehavior<RegisterDonorCommand, ResultViewModel<int>>
{
    private readonly IBloodDonorsRepository _repository;
    private readonly IAddressService _addressService;

    public ValidateRegisterDonorCommandBehavior(IBloodDonorsRepository repository, IAddressService addressService)
    {
        _repository = repository;
        _addressService = addressService;
    }

    public async Task<ResultViewModel<int>> Handle(RegisterDonorCommand request, RequestHandlerDelegate<ResultViewModel<int>> next, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        
        var emailAlreadyRegistered = await _repository.Exists(request.Email);

        if (emailAlreadyRegistered)
        {
          errors.Add($"Email {request.Email} already registered");
          return ResultViewModel<int>.Error(errors);
        }
        
        var addressValidationResult = await _addressService.ValidateAddressAsync(request.Zipcode);

        if (addressValidationResult.IsValid) return await next();
        
        errors.Add($"Invalid Address");
        return ResultViewModel<int>.Error(errors);

    }
}