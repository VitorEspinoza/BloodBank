using BloodBank.Core.Entities;
using BloodBank.Core.Models;
using BloodBank.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Persistence.Repositories;

public class BloodDonorsRepository : IBloodDonorsRepository
{
    private readonly BloodBankDbContext _context;

    public BloodDonorsRepository(BloodBankDbContext context)
    {
        _context = context;
    }

    public async Task<List<BloodDonor>> GetAllAsync()
    {
        var bloodDonors = await _context.BloodDonors.AsNoTracking().ToListAsync();
        return bloodDonors;
    }

    public async Task<BloodDonor?> GetById(int id)
    {
        var bloodDonor = await _context.BloodDonors
            .Include(bd => bd.Address)
            .Include(bd => bd.Donations)
            .SingleOrDefaultAsync(bd => bd.Id == id);
        return bloodDonor;
    }

    public async Task AddAsync(BloodDonor bloodDonor)
    {
        await _context.BloodDonors.AddAsync(bloodDonor);
    }

    public void Update(BloodDonor bloodDonor)
    {
        _context.Update(bloodDonor);
    }

    public async Task<bool> Exists(string email)
    {
        return await _context.BloodDonors.AnyAsync(bd => bd.Email == email);
    }
    
    public async Task<DonorEligibilityData> GetEligibilityDataAsync(int donorId)
    {
        var currentDate = DateTime.Now;
        var donor = await _context.BloodDonors
            .Where(d => d.Id == donorId)
            .Select(d => new
            {
                d.BirthDate,
                d.Weight,
                d.BiologicalSex,
                LastDonation = d.Donations.Max(donation => (DateTime?)donation.DonationDate)
            })
            .SingleOrDefaultAsync();

        if (donor == null)
            return new DonorEligibilityData(false);

        return new DonorEligibilityData(
            true,
            CalculatePreciseAge(donor.BirthDate, currentDate),
            donor.Weight,
            donor.LastDonation.HasValue
                ? (currentDate - donor.LastDonation.Value).Days
                : null,
            donor.BiologicalSex
        );
    }

    private static int CalculatePreciseAge(DateTime birthDate, DateTime referenceDate)
    {
        var age = referenceDate.Year - birthDate.Year;
    
        if (birthDate.Date > referenceDate.AddYears(-age)) 
            age--;

        return age;
    }
}
