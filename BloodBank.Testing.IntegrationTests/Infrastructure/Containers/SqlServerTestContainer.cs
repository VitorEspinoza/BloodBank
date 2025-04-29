using DotNet.Testcontainers.Builders;
using Testcontainers.MsSql;

namespace BloodBank.Testing.IntegrationTests.Infrastructure.Containers;

public static class SqlServerTestContainer 
{
    public static MsSqlContainer CreateContainer()
    {
        return  new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
            .WithPassword("P@ssw0rd123")
            .WithPortBinding(1433, true)
            .WithEnvironment("MSSQL_AGENT_ENABLED", "False") 
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilPortIsAvailable(1433)
                   )
            .Build();
    }
    
}