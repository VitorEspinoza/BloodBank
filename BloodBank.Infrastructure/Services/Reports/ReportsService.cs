using BloodBank.Core.Enums;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Infrastructure.Services.Reports.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Services.Reports;

public class ReportsService : IReportsService
{
    private readonly BloodBankDbContext _context;

    public ReportsService(BloodBankDbContext context)
    {
        _context = context;
    }

    public async Task<List<BloodQuantityByTypeReportData>> GetBloodQuantityByTypeAsync(CancellationToken cancellationToken)
    {
        var quantityBloodByType = await _context.BloodStocks
            .Select(bs => new BloodQuantityByTypeReportData() 
            {
                BloodType =   $"{bs.BloodType.Group}{(bs.BloodType.Rh == RhFactor.Positive ? "+" : "-")}",
                QuantityMl = bs.QuantityInMl,
            })
            .ToListAsync(cancellationToken: cancellationToken);

        return quantityBloodByType;
    }

    public async Task<List<DonationReportData>> GetRecentDonationsReportDataAsync(CancellationToken cancellationToken)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        
        var recentDonations = await _context.Donations
            .AsNoTracking()
            .Where(d => d.DonationDate >= cutoffDate)
            .Include(d => d.BloodDonor)
            .ThenInclude(d => d.Address)
            .Select(d => new DonationReportData() 
            {
                DonationDate = d.DonationDate,
                QuantityMl = d.QuantityInMl,
                DonorName = d.BloodDonor.FullName,
                BloodType = $"{d.BloodDonor.BloodType.Group}{(d.BloodDonor.BloodType.Rh == RhFactor.Positive ? "+" : "-")}",
                DonorCity = d.BloodDonor.Address.City,
                DonorState = d.BloodDonor.Address.State
            })
            .OrderByDescending(d => d.DonationDate)
            .ToListAsync(cancellationToken);

        return recentDonations;
    }
}