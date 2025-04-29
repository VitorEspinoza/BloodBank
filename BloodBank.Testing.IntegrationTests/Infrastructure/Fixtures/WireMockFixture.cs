using BloodBank.Testing.IntegrationTests.Infrastructure.Containers;
using WireMock.Client;
using WireMock.Net.Testcontainers;

namespace BloodBank.Testing.IntegrationTests.Infrastructure.Fixtures;

public class WireMockFixture : IAsyncLifetime
{
    public WireMockContainer MockHttpContainer { get; private set; }
    public IWireMockAdminApi client { get; private set; }
    
    public async Task InitializeAsync()
    {
        MockHttpContainer = WireMockTestContainer.CreateContainer();
        await MockHttpContainer.StartAsync();
        await WaitUntilContainerIsReady();
    }

    private async Task WaitUntilContainerIsReady()
    {
        var maxRetries = 10;
        var delay = TimeSpan.FromSeconds(1);
        
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                client = MockHttpContainer.CreateWireMockAdminClient();
                await client.GetMappingsAsync();
                return;
            }
            catch
            {
                await Task.Delay(delay);
            }
        }
    }
    
    public async Task DisposeAsync()
    {
            await MockHttpContainer.StopAsync();
            await MockHttpContainer.DisposeAsync();
    }
}