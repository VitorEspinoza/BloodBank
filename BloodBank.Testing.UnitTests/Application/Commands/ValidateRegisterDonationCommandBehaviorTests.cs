using BloodBank.Application.Commands.Donations.RegisterDonation;
using BloodBank.Application.ViewModels;
using BloodBank.Core.Enums;
using BloodBank.Core.Models;
using BloodBank.Core.Repositories;
using BloodBank.Core.Services;
using MediatR;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Application.Commands;

public class ValidateRegisterDonationCommandBehaviorTests
{
    private readonly IBloodDonorsRepository _repositoryMock;
    private readonly ValidateRegisterDonationCommandBehavior _behavior;
    private readonly RequestHandlerDelegate<ResultViewModel<int>> _nextDelegate;

    public ValidateRegisterDonationCommandBehaviorTests()
    {
        _repositoryMock = Substitute.For<IBloodDonorsRepository>();
        var eligibilityService = new DonorEligibilityService();
        _behavior = new ValidateRegisterDonationCommandBehavior(eligibilityService, _repositoryMock);
        _nextDelegate = Substitute.For<RequestHandlerDelegate<ResultViewModel<int>>>();
        _nextDelegate().Returns(new ResultViewModel<int>(42, true));
    }

    [Fact]
    public async Task DonorNotFound_DonationCommand_ReturnsError()
    {
        const int donorId = 999;
        _repositoryMock
            .GetEligibilityDataAsync(donorId)
            .Returns(new DonorEligibilityData(donorExists: false));

        var command = new RegisterDonationCommand { BloodDonorId = donorId };

        var result = await _behavior.Handle(command, _nextDelegate, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e.Contains("not found"));
        await _nextDelegate.DidNotReceive()();
    }

    [Fact]
    public async Task EligibleDonor_DonationCommand_ProceedsToNextHandler()
    {
        const int donorId = 1;
        _repositoryMock
            .GetEligibilityDataAsync(donorId)
            .Returns(new DonorEligibilityData(
                donorExists: true,
                age: 25,
                weight: 70,
                daysSinceLastDonation: 100,
                biologicalSex: BiologicalSex.Male));

        var command = new RegisterDonationCommand { BloodDonorId = donorId };

        var result = await _behavior.Handle(command, _nextDelegate, CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _nextDelegate.Received(1)();
    }

    [Fact]
    public async Task IneligibleDonor_DonationCommand_ReturnsErrorWithReasons()
    {
        const int donorId = 1;
        _repositoryMock
            .GetEligibilityDataAsync(donorId)
            .Returns(new DonorEligibilityData(
                donorExists: true,
                age: 17,
                weight: 49,
                daysSinceLastDonation: 30,
                biologicalSex: BiologicalSex.Male));

        var command = new RegisterDonationCommand { BloodDonorId = donorId };

        var result = await _behavior.Handle(command, _nextDelegate, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Equal(3, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.Contains("Minimum age: 18 years"));
        Assert.Contains(result.Errors, e => e.Contains("Minimum weight: 50kg"));
        Assert.Contains(result.Errors, e => e.Contains("Wait 60 days between donations"));

 
        await _nextDelegate.DidNotReceive()();
    }
}