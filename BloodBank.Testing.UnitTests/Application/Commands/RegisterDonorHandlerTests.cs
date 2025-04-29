using BloodBank.Application.Commands.Donors.RegisterDonor;
using BloodBank.Application.ViewModels;
using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using BloodBank.Testing.Common.Fakers;
using Bogus;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Application.Commands;
public class RegisterDonorHandlerTests
{
    private readonly IAddressService _addressServiceMock;
    private readonly IBloodDonorsRepository _repositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly RegisterDonorHandler _handler;

    public RegisterDonorHandlerTests()
    {
        _addressServiceMock = Substitute.For<IAddressService>();
        _repositoryMock = Substitute.For<IBloodDonorsRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
   
        _handler = new RegisterDonorHandler(_addressServiceMock, _repositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task ValidCommand_Handle_RegistersDonorAndReturnsSuccess()
    {
        var command = RegisterDonorCommandFaker.Generate();

        var addressResponse = BloodDonorFaker.GenerateAddress();
        _addressServiceMock.PersistAddressAsync(command.Zipcode, command.Number, command.Complement)
            .Returns(addressResponse);

        _unitOfWorkMock.ExecuteInTransactionAsync(Arg.Any<Func<Task<ResultViewModel<int>>>>())
            .Returns(callInfo =>
            {
                var func = callInfo.Arg<Func<Task<ResultViewModel<int>>>>();
                return func();
            });
       
        _repositoryMock.AddAsync(Arg.Any<BloodDonor>()).Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.IsType<int>(result.Data); 
        await _addressServiceMock.Received(1).PersistAddressAsync(command.Zipcode, command.Number, command.Complement);
        await _repositoryMock.Received(1).AddAsync(Arg.Any<BloodDonor>());
    }
}