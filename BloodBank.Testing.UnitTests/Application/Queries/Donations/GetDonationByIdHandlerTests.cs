using BloodBank.Application.Queries.Donations.GetDonationById;
using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using BloodBank.Testing.Common.Fakers;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Application.Queries.Donations;

public class GetDonationByIdHandlerTests
{
    private readonly IDonationRepository _repository;
    private readonly GetDonationByIdHandler _handler;

    public GetDonationByIdHandlerTests()
    {
        _repository = Substitute.For<IDonationRepository>();
        _handler = new GetDonationByIdHandler(_repository);
    }

    [Fact]
    public async Task DonationExist_Handle_ShouldReturnDonationViewModel()
    {
        var donation = DonationFaker.Generate();
        _repository.GetById(Arg.Any<int>()).Returns(donation);

        var result = await _handler.Handle(new GetDonationByIdQuery(donation.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(donation.QuantityInMl, result.Data.QuantityInMl);
        await _repository.Received(1).GetById(donation.Id);
    }

    [Fact]
    public async Task DonationNotExist_Handle_ShouldReturnNull()
    {
        _repository.GetById(Arg.Any<int>()).Returns((Donation?)null);

        var result = await _handler.Handle(new GetDonationByIdQuery(1), CancellationToken.None);

        Assert.Null(result);
    }
}