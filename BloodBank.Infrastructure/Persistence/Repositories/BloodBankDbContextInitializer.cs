using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Persistence.Repositories;

public class BloodBankDbContextInitializer
{
    private readonly BloodBankDbContext _context;
    
    public BloodBankDbContextInitializer(BloodBankDbContext context)
    {
        _context = context;
    }
    
    public async Task InitializeAsync()
    {
            await _context.Database.MigrateAsync();
            
            await SeedBloodStocksAsync();
    }
    
    private async Task SeedBloodStocksAsync()
    {
        if (await _context.BloodStocks.AnyAsync())
        {
            return;
        }
            
        var bloodStocks = new List<BloodStock>();
        
        foreach (var group in Enum.GetValues<BloodTypeGroup>())
        {
            foreach (var rh in Enum.GetValues<RhFactor>())
            {
                var bloodType = new BloodType(group, rh);
                bloodStocks.Add(BloodStock.CreateInitial(bloodType));
            }
        }
        
        await _context.BloodStocks.AddRangeAsync(bloodStocks);
        await _context.SaveChangesAsync();
    }
}