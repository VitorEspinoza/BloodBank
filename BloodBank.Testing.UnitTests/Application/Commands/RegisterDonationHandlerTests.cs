using BloodBank.Application.Commands.Donations.RegisterDonation;
using BloodBank.Application.ViewModels;
using BloodBank.Core.DomainEvents;
using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Testing.Common.Fakers;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BloodBank.Testing.UnitTests.Application.Commands;

public class RegisterDonationHandlerTests
{
    [Fact]
    public async Task InputDataAreOk_Registered_Success()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var bloodDonorsRepo = Substitute.For<IBloodDonorsRepository>();
        var donationsRepo = Substitute.For<IDonationRepository>();
        var outboxRepo = Substitute.For<IOutboxRepository>();
        
        var command = RegisterDonationCommandFaker.Generate();
        var fakeDonor = BloodDonorFaker.Generate();
        var fakeDonation = command.ToEntity(fakeDonor);
        
        bloodDonorsRepo.GetById(Arg.Any<int>())!.Returns(Task.FromResult(fakeDonor));

        unitOfWork.ExecuteInTransactionAsync(Arg.Any<Func<Task<ResultViewModel<int>>>>())
            .Returns(call =>
            {
                var func = call.Arg<Func<Task<ResultViewModel<int>>>>();
                return func();
            });
        var handler = new RegisterDonationHandler(unitOfWork, bloodDonorsRepo, donationsRepo, outboxRepo);

        Assert.NotNull(command);
        Assert.NotNull(command.ToEntity(fakeDonor));  
        Assert.NotNull(fakeDonor);
        Assert.NotNull(fakeDonation);
        var result = await handler.Handle(command, CancellationToken.None);

        
        Assert.True(result.IsSuccess);
        Assert.Equal(fakeDonation.Id, result.Data);
 
        await donationsRepo.Received(1).AddAsync(Arg.Is<Donation>(d => d.QuantityInMl == command.QuantityInMl));
        await outboxRepo.Received(1).SaveEvent(Arg.Any<IDomainEvent>());
    }
}