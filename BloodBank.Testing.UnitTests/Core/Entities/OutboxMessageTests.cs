using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Testing.Common.Fakers;
using Bogus;

namespace BloodBank.Testing.UnitTests.Core.Entities;
public class OutboxMessageTests
{
    
    [Fact]
    public void MarkAsProcessed_FromPending_UpdatesStatusAndTimestamp()
    {
        var message = OutboxFaker.GenerateOutboxMessage();

        message.MarkAsProcessed();

        Assert.Equal(OutboxMessageStatus.Processed, message.Status);
        Assert.NotNull(message.ProcessedAt);
        Assert.Null(message.LockedUntil);
    }

    [Fact]
    public void MarkAsFailed_FromAnyState_IncrementsRetryAndStoresError()
    {
        var message = OutboxFaker.GenerateOutboxMessage();
        const string error = "err";
        var previousRetries = message.RetryCount;

        message.MarkAsFailed(error);

        Assert.Equal(OutboxMessageStatus.Failed, message.Status);
        Assert.Equal(error, message.Error);
        Assert.Equal(previousRetries + 1, message.RetryCount);
        Assert.Null(message.LockedUntil);
    }

    [Fact]
    public void AcquireLock_FromPending_SetsLockedUntilAndStatusProcessing()
    {
        var message = OutboxFaker.GenerateOutboxMessage();
        var duration = TimeSpan.FromMinutes(10);

        message.AcquireLock(duration);

        Assert.Equal(OutboxMessageStatus.Processing, message.Status);
        Assert.NotNull(message.LockedUntil);
    }

    [Fact]
    public void ShouldRetry_BeWithRetriesBelowThreshold_ReturnsTrue()
    {
        var message = OutboxFaker.GenerateOutboxMessage();

        for (int i = 0; i < 2; i++)
            message.MarkAsFailed("err");

        Assert.True(message.ShouldRetry(5));
    }

    [Fact]
    public void ShouldRetry_BeWithRetriesAtOrAboveThreshold_ReturnsFalse()
    {
        var message = OutboxFaker.GenerateOutboxMessage();

        for (int i = 0; i < 5; i++)
            message.MarkAsFailed("err");

        Assert.False(message.ShouldRetry(5));
    }

    [Fact]
    public void ResetForRetry_AfterFailure_ResetsStatusAndSetsBackoffLock()
    {
        var message = OutboxFaker.GenerateOutboxMessage();
        message.MarkAsFailed("1");
        message.MarkAsFailed("2");

        message.ResetForRetry();

        Assert.Equal(OutboxMessageStatus.Pending, message.Status);
        Assert.NotNull(message.LockedUntil);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    public void CalculateBackoff_WithVariousRetryCounts_ReturnsExponentialDelay(int retryCount)
    {
        var message = OutboxFaker.GenerateOutboxMessage();
        for (int i = 0; i < retryCount; i++)
            message.MarkAsFailed("fail");

        var backoff = message.CalculateBackoff();
        var expected = Math.Pow(2, Math.Min(retryCount, 10));

        Assert.Equal(expected, backoff.TotalSeconds, precision: 1);
    }

    [Fact]
    public void MarkAsMovedToDLX_FromAnyState_SetsStatusToDeadLettered()
    {
        var message = OutboxFaker.GenerateOutboxMessage();

        message.MarkAsMovedToDlx();

        Assert.Equal(OutboxMessageStatus.DeadLettered, message.Status);
    }
   
}