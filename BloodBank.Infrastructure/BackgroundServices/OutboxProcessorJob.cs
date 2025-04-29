using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.MessageBus;
using BloodBank.Infrastructure.MessageBus.Interfaces;
using BloodBank.Infrastructure.MessageBus.TopologyConfig.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using Xunit.Abstractions;

namespace BloodBank.Infrastructure.BackgroundServices;

public class OutboxProcessorJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly OutboxSettings _settings;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    private readonly AsyncPolicyWrap _resiliencePolicy;

    
    public OutboxProcessorJob(
        IServiceProvider serviceProvider,
        IOptions<OutboxSettings> settings, ILogger<OutboxProcessorJob> logger)
    {
        _serviceProvider = serviceProvider;
        _settings = settings.Value;


        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: _settings.CircuitBreakerThreshold,
                durationOfBreak: TimeSpan.FromSeconds(_settings.CircuitBreakerResetSeconds));

        _resiliencePolicy = _circuitBreakerPolicy
            .WrapAsync(Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: _settings.ImmediateRetryCount,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), 60))));

    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessPendingMessages(cancellationToken);
          
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_settings.ProcessingIntervalSeconds), cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
    private async Task ProcessPendingMessages(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var topologyDefinition = scope.ServiceProvider.GetRequiredService<IEventBusTopologyDefinition>();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBusClient>();
        var pendingMessages = await outboxRepository.GetPendingMessagesAsync(_settings.BatchSize, cancellationToken);
        
        if (pendingMessages.Count == 0)
            return;

        var lockDuration = TimeSpan.FromSeconds(_settings.MessageLockTimeoutSeconds);

        foreach (var message in pendingMessages)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            
            message.AcquireLock(lockDuration);

            try
            {
                await outboxRepository.UpdateAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                continue;
            }


            try
            {
                if (_circuitBreakerPolicy.CircuitState == CircuitState.HalfOpen)
                    await messageBus.Publish("health.check", "ping", topologyDefinition.HealthCheckExchange);
            }
            catch
            {
                
            }
            
            try
            {
                await _resiliencePolicy.ExecuteAsync(async () =>
                    await messageBus.Publish(message.RoutingKey, message.Payload, message.Exchange)
                );

                message.MarkAsProcessed();
                await outboxRepository.UpdateAsync(message, cancellationToken);
            }
            catch (BrokenCircuitException)
            {
      
                message.ResetForRetry();
                await outboxRepository.UpdateAsync(message, cancellationToken);
                break;
            }
            catch (Exception ex)
            {
                message.MarkAsFailed(ex.Message);

                if (message.ShouldRetry(_settings.MessageRetryLimit))
                {
                    message.ResetForRetry();
                }
                else
                {
                    try 
                    {
                        await messageBus.Publish(
                            message.RoutingKey, 
                            message.Payload, 
                            topologyDefinition.DeadLetterExchange 
                        );
            
                        message.MarkAsMovedToDlx();
                    }
                    catch (Exception dlxEx)
                    { 
                        message.UpdateError($"Processing error. First: {ex.Message}, Second: Failed to move to DLX - {dlxEx.Message} - occurred at: {DateTime.UtcNow}");
                    }
                }
                
                await outboxRepository.UpdateAsync(message, cancellationToken);
            }
        }
    }
    


}