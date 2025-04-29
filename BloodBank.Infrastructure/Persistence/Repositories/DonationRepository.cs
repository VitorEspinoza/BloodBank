using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Persistence.Repositories;

public class DonationRepository : IDonationRepository
{
    private BloodBankDbContext _context;

    public DonationRepository(BloodBankDbContext context)
    {
        _context = context;
    }

    public async Task<List<Donation>> GetAll()
    {
        var donations = await _context.Donations
            .Include(d => d.BloodDonor)
            .ToListAsync();
        
        return donations;
    }
    
    public async Task<Donation?> GetById(int id)
    {
        var donation = await _context.Donations
            .AsNoTracking()
            .Include(d => d.BloodDonor)
            .SingleOrDefaultAsync(d => d.Id == id);
        
        return donation;
    }

    public async Task AddAsync(Donation donation)
    {
        await _context.Donations.AddAsync(donation);
    }
}