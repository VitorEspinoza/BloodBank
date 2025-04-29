using BloodBank.Application.Commands.Donors.UpdateDonor;
using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using BloodBank.Testing.Common.Fakers;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Application.Commands;

public class UpdateDonorHandlerTests
{
    private readonly IAddressService _addressServiceMock;
    private readonly IBloodDonorsRepository _repositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateDonorHandler _handler;
    public UpdateDonorHandlerTests()
    {
        _addressServiceMock = Substitute.For<IAddressService>();
        _repositoryMock = Substitute.For<IBloodDonorsRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _handler = new UpdateDonorHandler(_repositoryMock, _addressServiceMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task ValidCommandWithChangedAddress_Handle_UpdatesDonorWithNewAddress()
    {
        
        var command = UpdateDonorCommandFaker.Generate();

        var existingDonor = BloodDonorFaker.Generate();
        existingDonor.UpdateAddress(BloodDonorFaker.GenerateAddress());
        
        _repositoryMock.GetById(Arg.Any<int>()).Returns(existingDonor);
        
        _addressServiceMock.PersistAddressAsync(command.Zipcode, command.Number, command.Complement)
            .Returns(BloodDonorFaker.GenerateAddress());
        
        _unitOfWorkMock.ExecuteInTransactionAsync(Arg.Any<Func<Task<ResultViewModel>>>())
            .Returns(callInfo =>
            {
                var func = callInfo.Arg<Func<Task<ResultViewModel>>>();
                return func();
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _addressServiceMock.Received(1).PersistAddressAsync(command.Zipcode, command.Number, command.Complement);
        _repositoryMock.Received(1).Update(existingDonor);
        Assert.Equal(command.FullName, existingDonor.FullName);
        Assert.Equal(command.Email, existingDonor.Email);
        Assert.Equal(command.Weight, existingDonor.Weight);
    }

    [Fact]
    public async Task ValidCommandWithSameAddress_Handle_UpdatesDonorWithoutChangingAddress()
    {
        var command = UpdateDonorCommandFaker.Generate();
        
        var addressNotChanged = BloodDonorFaker.GenerateAddress();
        var existingDonor = BloodDonorFaker.Generate();
        existingDonor.UpdateAddress(addressNotChanged);
        
        command.Zipcode = addressNotChanged.ZipCode;
        command.Number = addressNotChanged.Number;
        command.Complement = addressNotChanged.Complement;

        _repositoryMock.GetById(Arg.Any<int>()).Returns(existingDonor);

        _unitOfWorkMock.ExecuteInTransactionAsync(Arg.Any<Func<Task<ResultViewModel>>>())
            .Returns(callInfo =>
            {
                var func = callInfo.Arg<Func<Task<ResultViewModel>>>();
                return func();
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _addressServiceMock.DidNotReceive()
            .PersistAddressAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        _repositoryMock.Received(1).Update(existingDonor);
    }

}