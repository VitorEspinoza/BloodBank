using BloodBank.Core.Entities;
using BloodBank.Core.Models;

namespace BloodBank.Core.Repositories;

public interface IBloodDonorsRepository
{
    Task<List<BloodDonor>> GetAllAsync();
    Task<BloodDonor?> GetById(int id);
    Task AddAsync(BloodDonor bloodDonor);
    void Update(BloodDonor bloodDonor);
    Task<bool> Exists(string email);
    
    Task<DonorEligibilityData> GetEligibilityDataAsync(int donorId);
}
