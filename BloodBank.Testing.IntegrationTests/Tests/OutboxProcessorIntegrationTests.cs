using BloodBank.Core.Enums;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.MessageBus;
using BloodBank.Infrastructure.MessageBus.Interfaces;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Testing.Common.Fakers;
using BloodBank.Testing.IntegrationTests.Infrastructure.Collections;
using BloodBank.Testing.IntegrationTests.Infrastructure.Fixtures;
using static BloodBank.Testing.IntegrationTests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace BloodBank.Testing.IntegrationTests.Tests;

[Collection(nameof(OutboxTestsCollection))]
public class OutboxProcessorIntegrationTests : IAsyncLifetime
{
    private readonly SharedTestFixture _fixture;
    private TestRabbitMqClient _testClient;
    
    public OutboxProcessorIntegrationTests(SharedTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.SetupHostAndInfra(services =>
        {
        var tempProvider = services.BuildServiceProvider();
            var realClient = tempProvider.GetRequiredService<IMessageBusClient>();
            _testClient =  new TestRabbitMqClient(realClient);
            services.RemoveAll<IMessageBusClient>();

            services.AddSingleton<IMessageBusClient>(_testClient);
        });
        
        await using var scope = _fixture.ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();

        await dbContext.OutboxMessages.ExecuteDeleteAsync();
        await dbContext.SaveChangesAsync();
        
    }

    [Fact]
    public async Task ValidMessage_Processing_ShouldPublishSuccessfully()
    {
        await using var scope = _fixture.ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var testEvent = DonationRegisteredFaker.Generate();
        await outboxRepository.SaveEvent(testEvent);
        await dbContext.SaveChangesAsync();

        var expirationTimeout = TimeSpan.FromSeconds(60);

        var messageProcessed = await WaitForConditionAsync(async () =>
        {
            var message = await dbContext.OutboxMessages
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return message != null &&
                   message.Status == OutboxMessageStatus.Processed;
        }, expirationTimeout);

        Assert.True(messageProcessed, "The message should have been successfully processed and published on the message bus");
        Assert.Equal(0, _testClient.FailureCount);
        Assert.Equal(1, _testClient.PublishAttempts);
    }
    
    [Fact]
    public async Task TemporaryFailure_ProcessingWithRetries_ShouldEventuallySucceed()
    {
        await using var scope = _fixture.ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>(); 
        var outboxSettings = scope.ServiceProvider.GetRequiredService<IOptions<OutboxSettings>>().Value;

        var failureQuantity = outboxSettings.ImmediateRetryCount - 1;
        _testClient.SetupFailureScenario(failureQuantity);
        var testEvent = DonationRegisteredFaker.Generate();
        
        await outboxRepository.SaveEvent(testEvent);
        await dbContext.SaveChangesAsync();
        
        var expirationTimeout = TimeSpan.FromSeconds(60);
        
        var messageProcessed = await WaitForConditionAsync(async () => 
        {
            var message = await dbContext.OutboxMessages
                .AsNoTracking()
                .FirstOrDefaultAsync();
                    
            return message != null && message.Status == OutboxMessageStatus.Processed;
        }, expirationTimeout);
        
        Assert.True(messageProcessed, "The message should be processed after retry attempts");
        Assert.Equal(failureQuantity, _testClient.FailureCount);
        Assert.Equal(_testClient.PublishAttempts, failureQuantity + 1);
    }

    [Fact]
    public async Task PersistentFailures_ExceedingRetryLimit_ShouldMoveToDeadLetterQueue()
    {
        await using var scope = _fixture.ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var outboxSettings = scope.ServiceProvider.GetRequiredService<IOptions<OutboxSettings>>().Value;
        
        var totalFailures = (outboxSettings.ImmediateRetryCount + 1) * outboxSettings.MessageRetryLimit;
        
        _testClient.SetupFailureScenario(totalFailures); 
        var testEvent = DonationRegisteredFaker.Generate();
        await outboxRepository.SaveEvent(testEvent);
        await dbContext.SaveChangesAsync();
        
        var expirationTimeout = TimeSpan.FromSeconds(60);
        
        var messageSentToDlx = await WaitForConditionAsync(async () =>
        {
            var message = await dbContext.OutboxMessages
                .AsNoTracking()
                .FirstOrDefaultAsync();
                    
            return message != null && message.Status == OutboxMessageStatus.DeadLettered;
        }, expirationTimeout);
        Assert.True(messageSentToDlx, "The message should be moved to the DLX after exceeding retry limit");
    }
    
    [Fact]
    public async Task MultipleFailures_CircuitBreakerOpens_ShouldPauseAndResumeProcessing()
    {
        await using var scope = _fixture.ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var outboxSettings = scope.ServiceProvider.GetRequiredService<IOptions<OutboxSettings>>().Value;
        
        _testClient.SetupFailureScenario(outboxSettings.CircuitBreakerThreshold * (outboxSettings.ImmediateRetryCount + 1));

        var messagesNeeded = (int)Math.Ceiling((double)outboxSettings.CircuitBreakerThreshold / outboxSettings.MessageRetryLimit) + 1;
        for (var i = 0; i < messagesNeeded; i++)
        {
            var testEvent = DonationRegisteredFaker.Generate();
            await outboxRepository.SaveEvent(testEvent);
        }
    
        await dbContext.SaveChangesAsync();
        
        var messagesProcesseds = await WaitForConditionAsync(async () => {
            var processedCount = await dbContext.OutboxMessages
                .AsNoTracking()
                .CountAsync(m => m.Status == OutboxMessageStatus.Processed);
            
            return processedCount == messagesNeeded;
        }, TimeSpan.FromSeconds(60));
        
        Assert.True(messagesProcesseds);
        
    }


    public async Task DisposeAsync()
    {
     
    }

}
