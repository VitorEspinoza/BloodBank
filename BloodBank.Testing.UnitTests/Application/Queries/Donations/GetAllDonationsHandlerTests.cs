using BloodBank.Application.Queries.Donations.GetAllDonations;
using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using BloodBank.Testing.Common.Fakers;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Application.Queries.Donations;

public class GetAllDonationsHandlerTests
{
    private readonly IDonationRepository _repository;
    private readonly GetAllDonationsHandler _handler;

    public GetAllDonationsHandlerTests()
    {
        _repository = Substitute.For<IDonationRepository>();
        _handler = new GetAllDonationsHandler(_repository);
    }

    [Fact]
    public async Task DonationsExist_Handle_ShouldReturnMappedDonations()
    {
        var donations = DonationFaker.GenerateList(5);
        _repository.GetAll().Returns(donations);
        
        var result = await _handler.Handle(new GetAllDonationsQuery(), CancellationToken.None);
        
        Assert.Equal(5, result.Data.Count);
        Assert.All(result.Data, vm => Assert.Contains(donations, d => d.Id == vm.Id));
        await _repository.Received(1).GetAll();
    }

    [Fact]
    public async Task DonationsNotExist_Handle_ShouldReturnEmptyList()
    {
        _repository.GetAll().Returns(new List<Donation>());
        
        var result = await _handler.Handle(new GetAllDonationsQuery(), CancellationToken.None);
        
        Assert.Empty(result.Data);
    }
}