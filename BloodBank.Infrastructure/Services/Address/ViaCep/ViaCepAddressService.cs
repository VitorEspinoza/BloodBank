using System.Net.Http.Json;
using System.Text.Json;
using BloodBank.Core.Repositories;
using CoreAddress = BloodBank.Core.Entities.Address;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using BloodBank.Infrastructure.Services.Address.ViaCep.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BloodBank.Infrastructure.Services.Address.ViaCep;

public class ViaCepAddressService : IAddressService
{
    private const string CacheKeyPrefix = "ViaCep_";
    private const int CacheDurationMinutes = 30;

    private readonly HttpClient _httpClient;
    private readonly IAddressRepository _addressRepository;
    private readonly IMemoryCache _cache;
    
    public ViaCepAddressService(HttpClient httpClient, IAddressRepository addressRepository, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _addressRepository = addressRepository;
        _cache = cache;
    }

    private async Task<ViaCepResponse?> GetAddressDataAsync(string zipcode)
    {
        var cacheKey = $"{CacheKeyPrefix}{zipcode}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            try
            {
                var response = await _httpClient.GetAsync($"{zipcode}/json/");
                response.EnsureSuccessStatusCode();
                var viaCepData = await response.Content.ReadFromJsonAsync<ViaCepResponse>();
            
                if (!string.IsNullOrEmpty(viaCepData?.Erro))
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromHours(24));
                    return viaCepData;
                }

                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheDurationMinutes)); 
                return viaCepData;
            }
            catch (HttpRequestException ex)
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(10)); 
                return null;
            }
        });
    }
    
    private async Task<CoreAddress?> MapToAddress(string zipcode, string number, string? complement)
    {
        var viaCepData = await GetAddressDataAsync(zipcode);
        
        return new CoreAddress(
            zipCode: zipcode,
            street: viaCepData.Logradouro,
            neighborhood:viaCepData.Bairro,
            city: viaCepData.Localidade,
            state: viaCepData.Uf,
            complement: complement,
            number: number
        );
    }
    
    public async Task<CoreAddress> PersistAddressAsync(string zipcode, string number, string? complement)
    {
        var existingAddress = await _addressRepository.GetByCepAndNumberAndComplementAsync(zipcode, number, complement);

        if (existingAddress is not null) return existingAddress;
        
        var newAddress = await MapToAddress(zipcode, number, complement);
        
        await _addressRepository.AddAsync(newAddress!);
        
        return newAddress!;
    }
    public async Task<AddressValidationResult> ValidateAddressAsync(string zipcode)
    {
        try
        {
            var viaCepData = await GetAddressDataAsync(zipcode);
            
            if (viaCepData != null && !string.IsNullOrEmpty(viaCepData.Erro))
                return new AddressValidationResult(false, AddressValidationError.InvalidZipcode);
            
            if (viaCepData != null)
                return new AddressValidationResult(true);
            
            return new AddressValidationResult(false, AddressValidationError.ApiFailure);
        }
        catch (Exception ex) 
        {
            return new AddressValidationResult(false, AddressValidationError.UnknownError);
        }
    }
}


