using BloodBank.Core.Entities;

namespace BloodBank.Core.Repositories;

public interface IAddressRepository
{
    Task AddAsync(Address address);
    
    Task<Address?> GetByCepAndNumberAndComplementAsync(string zipcode, string number, string? complement);
}