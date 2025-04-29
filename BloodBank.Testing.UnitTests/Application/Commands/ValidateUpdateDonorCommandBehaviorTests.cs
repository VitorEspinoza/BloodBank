using BloodBank.Application.Commands.Donors.UpdateDonor;
using BloodBank.Application.ViewModels;
using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Services.Address;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using BloodBank.Testing.Common.Fakers;
using MediatR;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Application.Commands;

public class ValidateUpdateDonorCommandBehaviorTests
{
    private readonly IAddressService  _addressService;
    private readonly IBloodDonorsRepository  _bloodDonorsRepository;
    private readonly ValidateUpdateDonorCommandBehavior _behavior;
    private readonly CancellationToken _cancellationToken;
    private readonly RequestHandlerDelegate<ResultViewModel> _nextDelegate; 
    
    public ValidateUpdateDonorCommandBehaviorTests()
    {
        _addressService = Substitute.For<IAddressService>();
        _bloodDonorsRepository = Substitute.For<IBloodDonorsRepository>();
        _behavior = new ValidateUpdateDonorCommandBehavior(_bloodDonorsRepository, _addressService);
        _cancellationToken = CancellationToken.None;
        _nextDelegate = Substitute.For<RequestHandlerDelegate<ResultViewModel>>();

        _nextDelegate().Returns(ResultViewModel.Success());
    }

    [Fact]
    public async Task validData_UpdateDonorCommand_ProceedsToNextHandler()
    {
        var existingBloodDonor = BloodDonorFaker.Generate();
        _bloodDonorsRepository.GetById(Arg.Any<int>())!.Returns(Task.FromResult(existingBloodDonor));
        _bloodDonorsRepository.Exists(Arg.Any<string>()).Returns(false);
        _addressService.ValidateAddressAsync(Arg.Any<string>())
            .Returns(new AddressValidationResult(true));
        
        var command = UpdateDonorCommandFaker.Generate();
        
        var result = await _behavior.Handle(command, _nextDelegate, _cancellationToken);
        
        Assert.True(result.IsSuccess);
        await _nextDelegate.Received(1)();
    }
    
    [Fact]  
    public async Task EmailAndAddressUnchanged_UpdateDonorCommand_NoValidationAndProceeds()  
    {  
        var existingDonor = BloodDonorFaker.Generate();
        var address = BloodDonorFaker.GenerateAddress();
        existingDonor.UpdateAddress(address);
        var command = UpdateDonorCommandFaker.Generate();  
    
        command.Email = existingDonor.Email;  
        command.Zipcode = existingDonor.Address.ZipCode;  
        command.Complement = existingDonor.Address.Complement;
        command.Number = existingDonor.Address.Number;
    
        _bloodDonorsRepository.GetById(Arg.Any<int>()).Returns(existingDonor);  
  
        var result = await _behavior.Handle(command, _nextDelegate, _cancellationToken);  
  
        await _addressService.DidNotReceive().ValidateAddressAsync(Arg.Any<string>());  
        await _bloodDonorsRepository.DidNotReceive().Exists(Arg.Any<string>());  
        Assert.True(result.IsSuccess);  
    }  
    [Fact]
    public async Task BloodDonorNotFound_UpdateDonorCommand_ReturnsError()
    {
        _bloodDonorsRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult<BloodDonor?>(null));

        var command = UpdateDonorCommandFaker.Generate();
        
        await _behavior.Handle(command, _nextDelegate, _cancellationToken);
        
        await _bloodDonorsRepository.DidNotReceive().Exists(Arg.Any<string>());
        await _nextDelegate.DidNotReceive()();

    }

    [Fact]
    public async Task emailAlreadyRegistered_UpdateDonorCommand_ReturnsError()
    {
        _bloodDonorsRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult<BloodDonor?>(BloodDonorFaker.Generate()));
        _bloodDonorsRepository.Exists(Arg.Any<string>()).Returns(true);
         
        var command = UpdateDonorCommandFaker.Generate();
        
        var result = await _behavior.Handle(command, _nextDelegate, _cancellationToken);
        
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e.Contains("already registered"));
        Assert.Contains(result.Errors, e => e.Contains(command.Email));
        await _nextDelegate.DidNotReceive()();
    }

    [Fact]
    public async Task invalidAddress_UpdateDonorCommand_ReturnsError()
    {
        var existingBloodDonor = BloodDonorFaker.Generate();
        
        _bloodDonorsRepository.GetById(Arg.Any<int>()).Returns(Task.FromResult<BloodDonor?>(existingBloodDonor));
        _bloodDonorsRepository.Exists(Arg.Any<string>()).Returns(false);
        _addressService.ValidateAddressAsync(Arg.Any<string>())
            .Returns(new AddressValidationResult(false));
        
        var command = UpdateDonorCommandFaker.Generate();
        
        var result = await _behavior.Handle(command, _nextDelegate, _cancellationToken);
        
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e.Contains("Invalid Address"));
        await _nextDelegate.DidNotReceive()();
    }
    
    
}