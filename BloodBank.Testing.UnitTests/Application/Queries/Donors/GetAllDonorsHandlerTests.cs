using BloodBank.Application.Queries.Donors.GetAllDonors;
using BloodBank.Core.Repositories;
using BloodBank.Testing.Common.Fakers;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Application.Queries.Donors;

public class GetAllDonorsHandlerTests
{
    private readonly IBloodDonorsRepository _repository;
    private readonly GetAllDonorsHandler _handler;

    public GetAllDonorsHandlerTests()
    {
        _repository = Substitute.For<IBloodDonorsRepository>();
        _handler = new GetAllDonorsHandler(_repository);
    }

    [Fact]
    public async Task DonorsExist_Handle_ShouldReturnAllDonors()
    {
        var donors = BloodDonorFaker.GenerateList(5);
        _repository.GetAllAsync().Returns(donors);
        
        var result = await _handler.Handle(new GetAllDonorsQuery(), CancellationToken.None);
        
        Assert.Equal(5, result.Data.Count);
        await _repository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task DonorsNotExist_Handle_ShouldReturnEmptyList()
    {
        _repository.GetAllAsync().Returns([]);

        var result = await _handler.Handle(new GetAllDonorsQuery(), CancellationToken.None);
        Assert.Empty(result.Data);
    }
}