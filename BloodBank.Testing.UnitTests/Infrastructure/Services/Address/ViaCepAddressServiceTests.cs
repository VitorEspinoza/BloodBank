using System.Net;
using System.Text.Json;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Services.Address;
using BloodBank.Infrastructure.Services.Address.ViaCep;
using BloodBank.Infrastructure.Services.Address.ViaCep.Models;
using BloodBank.Testing.Common.Fakers;
using Bogus;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using RichardSzalay.MockHttp;
using CoreAddress = BloodBank.Core.Entities.Address;
namespace BloodBank.Testing.UnitTests.Infrastructure.Services.Address;

public class ViaCepAddressServiceTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IAddressRepository _addressRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly ViaCepAddressService _service;
    private readonly Faker _faker;

    
    public ViaCepAddressServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://viacep.com.br/ws/");
        _addressRepository = Substitute.For<IAddressRepository>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _service = new ViaCepAddressService(httpClient, _addressRepository, _memoryCache);
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public async Task ValidZipcode_ValidatingAddress_ReturnsValidResult()
    {
        const string zipcode = "01001000";
        var responseContent = CreateValidViaCepResponse(zipcode);

        _mockHttp.When($"https://viacep.com.br/ws/{zipcode}/json/")
                 .Respond("application/json", JsonSerializer.Serialize(responseContent));

        var result = await _service.ValidateAddressAsync(zipcode);
        
        Assert.True(result.IsValid);
        Assert.Null(result.Error);
    }

    [Fact]
    public async Task InvalidZipcode_ValidatingAddress_ReturnsInvalidResult()
    {
        const string zipcode = "00000000";
        var errorResponse = CreateErrorViaCepResponse(zipcode);

        _mockHttp.When($"https://viacep.com.br/ws/{zipcode}/json/")
                 .Respond("application/json", JsonSerializer.Serialize(errorResponse));

        var result = await _service.ValidateAddressAsync(zipcode);

        Assert.False(result.IsValid);
        Assert.Equal(AddressValidationError.InvalidZipcode, result.Error);
    }
    
    [Fact]
    public async Task NewValidAddress_PersistingAddress_PersistsAndReturnsNewAddress()
    {
        const string zipcode = "01001000";
        const string number = "100";
        const string complement = "Apto 42";
        var responseContent = CreateValidViaCepResponse(zipcode);

        _mockHttp.When($"https://viacep.com.br/ws/{zipcode}/json/")
            .Respond("application/json", JsonSerializer.Serialize(responseContent));

        _addressRepository.GetByCepAndNumberAndComplementAsync(zipcode, number, complement)
            .Returns((CoreAddress)null);

        var result = await _service.PersistAddressAsync(zipcode, number, complement);

        Assert.NotNull(result);
        Assert.Equal(zipcode, result.ZipCode);
        Assert.Equal(number, result.Number);
        Assert.Equal(complement, result.Complement);
        Assert.Equal(responseContent.Logradouro, result.Street);
        Assert.Equal(responseContent.Bairro, result.Neighborhood);
        Assert.Equal(responseContent.Localidade, result.City);
        Assert.Equal(responseContent.Uf, result.State);

        await _addressRepository.Received(1).AddAsync(Arg.Is<CoreAddress>(
            a => a.ZipCode == zipcode && a.Number == number && a.Complement == complement));
    }

    [Fact]
    public async Task SameZipcode_FetchingAddressDataMultipleTimes_CachesResponse()
    {
        const string zipcode = "01001000"; 
        var responseContent = CreateValidViaCepResponse(zipcode);

        var request = _mockHttp.When($"https://viacep.com.br/ws/{zipcode}/json/")
            .Respond("application/json", JsonSerializer.Serialize(responseContent));

        await _service.ValidateAddressAsync(zipcode);
        await _service.ValidateAddressAsync(zipcode);

        Assert.Equal(1, _mockHttp.GetMatchCount(request));
    }

    
    [Fact]
    public async Task ApiFailure_ValidatingAddress_ReturnsApiFailureError()
    {
        const string zipcode = "01001000";

     _mockHttp.When($"https://viacep.com.br/ws/{zipcode}/json/")
             .Respond(req => throw new HttpRequestException("Simulated API failure", null, HttpStatusCode.InternalServerError));

        var result = await _service.ValidateAddressAsync(zipcode);

        Assert.False(result.IsValid);
        Assert.Equal(AddressValidationError.ApiFailure, result.Error);
    }

   
    [Fact]
    public async Task ExistingAddress_PersistingAddress_ReturnsExistingAddress()
    {
        const string zipcode = "01001000";
        const string number = "100";
        const string complement = "Apto 42";
        var existingAddress = BloodDonorFaker.GenerateAddress();

        _addressRepository.GetByCepAndNumberAndComplementAsync(zipcode, number, complement)
                          .Returns(existingAddress);

        var result = await _service.PersistAddressAsync(zipcode, number, complement);

        Assert.Same(existingAddress, result);
        await _addressRepository.DidNotReceive().AddAsync(Arg.Any<CoreAddress>());
    }


    private ViaCepResponse CreateValidViaCepResponse(string zipcode)
    {
        return new ViaCepResponse(
            Cep : zipcode,
            Logradouro : _faker.Address.StreetName(),
            Bairro : _faker.Address.County(),
            Localidade: _faker.Address.City(),
            Uf: _faker.Address.StateAbbr(),
            Erro: ""
        );
    }
    
    private ViaCepResponse CreateErrorViaCepResponse(string zipcode)
    {
        return new ViaCepResponse(
            Cep : zipcode,
            Logradouro : "",
            Bairro : "",
            Localidade: "",
            Uf: "",
            Erro: "true"
        );
    }
}
