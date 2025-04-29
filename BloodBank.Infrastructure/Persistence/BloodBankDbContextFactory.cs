using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BloodBank.Infrastructure.Persistence;

public class BloodBankDbContextFactory : IDesignTimeDbContextFactory<BloodBankDbContext>
{
    public BloodBankDbContext CreateDbContext(string[] args)
    {
        const string userSecretsId = "1c40043b-8f88-413a-b164-60a2d5f953a6";
        
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<BloodBankDbContext>()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BloodBankDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("BloodBankCs"));

        return new BloodBankDbContext(optionsBuilder.Options);
    }
}