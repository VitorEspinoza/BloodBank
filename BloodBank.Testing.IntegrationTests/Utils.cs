using Polly;

namespace BloodBank.Testing.IntegrationTests;

public static class Utils
{
    public static async Task<bool> WaitForConditionAsync(Func<Task<bool>> condition, TimeSpan timeout)
    {
        const int minDesiredRetries = 15;
    
        var maxIntervalMs = timeout.TotalMilliseconds / minDesiredRetries;
    
        var retryPolicy = Policy
            .HandleResult<bool>(result => !result)
            .WaitAndRetryAsync(
                retryCount: int.MaxValue, 
                sleepDurationProvider: retryAttempt => 
                {
                    var delay = Math.Min(
                        100 * Math.Pow(1.5, retryAttempt),  
                        maxIntervalMs);                     
                
                    return TimeSpan.FromMilliseconds(delay);
                });

        try
        {
            return await retryPolicy.ExecuteAsync(async () => await condition());
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}