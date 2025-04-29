using CoreAddress = BloodBank.Core.Entities.Address;

namespace BloodBank.Infrastructure.Services.Address.Interfaces;

public interface IAddressService
{
    Task<CoreAddress> PersistAddressAsync(string zipcode, string number, string? complement);
    Task<AddressValidationResult> ValidateAddressAsync(string zipcode);
}