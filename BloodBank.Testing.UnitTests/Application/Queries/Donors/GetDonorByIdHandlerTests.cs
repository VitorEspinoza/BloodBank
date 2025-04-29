using BloodBank.Application.Queries.Donors.GetDonorById;
using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using BloodBank.Testing.Common.Fakers;
using NSubstitute;

namespace BloodBank.Testing.UnitTests.Application.Queries.Donors;

public class GetDonorByIdHandlerTests
{
    private readonly IBloodDonorsRepository _repository;
    private readonly GetDonorByIdHandler _handler;

    public GetDonorByIdHandlerTests()
    {
        _repository = Substitute.For<IBloodDonorsRepository>();
        _handler = new GetDonorByIdHandler(_repository);
    }

    [Fact]
    public async Task DonorExists_Handle_ShouldReturnDonorViewModel()
    {
        var donor = BloodDonorFaker.Generate();
        _repository.GetById(Arg.Any<int>()).Returns(donor);
        
        var result = await _handler.Handle(new GetDonorByIdQuery(donor.Id), CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(donor.FullName, result.Data.FullName);
        await _repository.Received(1).GetById(donor.Id);
    }

    [Fact]
    public async Task DonorNotExist_Handle_ShouldReturnNull()
    {
        _repository.GetById(Arg.Any<int>()).Returns((BloodDonor?)null);
        
        var result = await _handler.Handle(new GetDonorByIdQuery(1), CancellationToken.None);

        Assert.Null(result);
    }
}