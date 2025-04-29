using BloodBank.Infrastructure.MessageBus.Interfaces;
using Xunit.Abstractions;

public class TestRabbitMqClient : IMessageBusClient, IAsyncDisposable
{
    private readonly IMessageBusClient _innerClient;
    private int _remainingFailures;
    private Exception _exceptionToThrow;
    public int FailureCount { get; private set; }
    public int PublishAttempts { get; private set; }
    
    public TestRabbitMqClient(IMessageBusClient innerClient)
    {
        _innerClient = innerClient;
        _exceptionToThrow = new Exception("Simulated RabbitMQ failure");
    }
    
    public void SetupFailureScenario(int failureCount, Exception exceptionToThrow = null)
    {
        _remainingFailures = failureCount;
        if (exceptionToThrow != null)
            _exceptionToThrow = exceptionToThrow;
        FailureCount = 0;
        PublishAttempts = 0;
    }
    
    public async Task Publish(string routingKey, string payload, string exchange)
    {
        PublishAttempts++;
        
        if (_remainingFailures > 0)
        {
            _remainingFailures--;
            FailureCount++;
            throw _exceptionToThrow;
        }
        
        await _innerClient.Publish(routingKey, payload, exchange);

    }
    
    public async ValueTask DisposeAsync()
    {
       if (_innerClient is IAsyncDisposable disposable)
       {
           try
           {
               await disposable.DisposeAsync();
           }
           catch (Exception ex)
           {
               Console.WriteLine($"Error in RabbitMQ dispose: {ex.Message}");
           }
       }
    }
}