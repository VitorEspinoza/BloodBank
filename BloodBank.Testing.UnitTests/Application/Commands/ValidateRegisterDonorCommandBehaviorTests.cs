using BloodBank.Application.Commands.Donors.RegisterDonor;
using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Services.Address;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using BloodBank.Testing.Common.Fakers;
using MediatR;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Application.Commands;

public class ValidateRegisterDonorCommandBehaviorTests
{
    private readonly IBloodDonorsRepository _repositoryMock;
    private readonly IAddressService _addressServiceMock;
    private readonly ValidateRegisterDonorCommandBehavior _behavior;
    private readonly RequestHandlerDelegate<ResultViewModel<int>> _nextDelegate;
    private readonly CancellationToken _cancellationToken;

    public ValidateRegisterDonorCommandBehaviorTests()
    {
        _repositoryMock = Substitute.For<IBloodDonorsRepository>();
        _addressServiceMock = Substitute.For<IAddressService>();
        _behavior = new ValidateRegisterDonorCommandBehavior(_repositoryMock, _addressServiceMock);
        _cancellationToken = CancellationToken.None;
        _nextDelegate = Substitute.For<RequestHandlerDelegate<ResultViewModel<int>>>();
        
        _nextDelegate().Returns(new ResultViewModel<int>(42, true));
    }

    [Fact]
    public async Task ValidAddress_RegisterDonorCommand_ProceedsToNextHandler()
    {
        var command = RegisterDonorCommandFaker.Generate();
        
        _repositoryMock.Exists(command.Email).Returns(false);
        _addressServiceMock.ValidateAddressAsync(command.Zipcode)
            .Returns(new AddressValidationResult(true));
        
        var result = await _behavior.Handle(command, _nextDelegate, _cancellationToken);
        
        Assert.True(result.IsSuccess);
        await _nextDelegate.Received(1)();
    }

    [Fact]
    public async Task EmailAlreadyRegistered_RegisterDonorCommand_ReturnsError()
    {
        var command = RegisterDonorCommandFaker.Generate();
        
        _repositoryMock.Exists(command.Email).Returns(true);
        
        var result = await _behavior.Handle(command, _nextDelegate, _cancellationToken);
        
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e.Contains("already registered"));
        Assert.Contains(result.Errors, e => e.Contains(command.Email));
        await _nextDelegate.DidNotReceive()();
    }

    [Fact]
    public async Task InvalidAddress_RegisterDonorCommand_ReturnsError()
    {
        var command = RegisterDonorCommandFaker.Generate();
        
        _repositoryMock.Exists(command.Email).Returns(false);
        _addressServiceMock.ValidateAddressAsync(command.Zipcode)
            .Returns(new AddressValidationResult( false, AddressValidationError.InvalidZipcode));
        
        var result = await _behavior.Handle(command, _nextDelegate, _cancellationToken);
        
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e.Contains("Invalid Address"));
        await _nextDelegate.DidNotReceive()();
    }
}