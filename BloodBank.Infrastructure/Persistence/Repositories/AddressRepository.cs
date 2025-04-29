using BloodBank.Core.Entities;
using BloodBank.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Persistence.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly BloodBankDbContext _context;

    public AddressRepository(BloodBankDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Address address)
    {
        await _context.Addresses.AddAsync(address);
    }
    

    public async Task<Address?> GetByCepAndNumberAndComplementAsync(string zipcode, string number, string? complement)
    {
        return await _context.Addresses
            .SingleOrDefaultAsync(a => a.ZipCode == zipcode && a.Number == number  && a.Complement == complement);
    }
}