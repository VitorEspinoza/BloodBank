using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.ValueObjects;

namespace BloodBank.Core.Repositories;

public interface IBloodStockRepository
{
    
    Task<List<BloodStock>> GetBloodStockSummaryReportAsync();
    Task<BloodStock> GetByTypeAsync(BloodType bloodType);

    Task UpdateAsync(BloodStock bloodStock);
    Task<bool> Exists(int id);
}