using BloodBank.Core.Entities;

namespace BloodBank.Core.Repositories;

public interface IDonationRepository
{
    Task<List<Donation>> GetAll();
    Task<Donation?> GetById(int id);
    Task AddAsync(Donation donation);
}