using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using BloodBank.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Persistence.Repositories;

public class BloodStockRepository : IBloodStockRepository
{
    private readonly BloodBankDbContext _context;

    public BloodStockRepository(BloodBankDbContext context)
    {
        _context = context;
    }

    public async Task<List<BloodStock>> GetBloodStockSummaryReportAsync()
    {
        var bloodStocks = await _context.BloodStocks.AsNoTracking().ToListAsync();
        return bloodStocks;
    }

    public async Task<BloodStock> GetByTypeAsync(BloodType bloodType)
    {
        var bloodStock = await _context.BloodStocks
            .AsNoTracking()
            .SingleOrDefaultAsync(bs => 
                bs.BloodType.Group == bloodType.Group && 
                bs.BloodType.Rh == bloodType.Rh);
       return bloodStock;
    }
    
    public async Task UpdateAsync(BloodStock bloodStock)
    {
        _context.BloodStocks.Update(bloodStock);
    }

    public async Task<bool> Exists(int id)
    {
        return await _context.BloodStocks.AnyAsync(bs => bs.Id == id);
    }
}