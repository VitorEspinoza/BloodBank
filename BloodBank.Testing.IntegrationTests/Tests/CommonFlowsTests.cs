using BloodBank.Application.Commands.Donations.RegisterDonation;
using BloodBank.Application.Commands.Donors.RegisterDonor;
using BloodBank.Application.Subscribers;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Repositories;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Infrastructure.Services.Address.Interfaces;
using BloodBank.Infrastructure.Services.Address.ViaCep;
using BloodBank.Infrastructure.Services.Notification.Brevo;
using BloodBank.Infrastructure.Services.Notification.Interfaces;
using BloodBank.Testing.Common.Fakers;
using BloodBank.Testing.IntegrationTests.Infrastructure.Collections;
using BloodBank.Testing.IntegrationTests.Infrastructure.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static BloodBank.Testing.IntegrationTests.Utils;
using WireMock.Admin.Mappings;
using WireMock.Client;

namespace BloodBank.Testing.IntegrationTests.Tests;

[Collection(nameof(CommonDependenciesCollection))]
public class CommonFlowsTests : IClassFixture<WireMockFixture>, IAsyncLifetime
{
    private readonly SharedTestFixture _sharedFixture;
    private readonly WireMockFixture _wireMockFixture;
    private IWireMockAdminApi _wireMockClient;
    private const string BrevoUri = "/smtp/email";
    public CommonFlowsTests(SharedTestFixture fixture, WireMockFixture wireMockFixture)
    {
        _sharedFixture = fixture;
        _wireMockFixture = wireMockFixture;
    }
    
    public async Task InitializeAsync()
    {
        await _sharedFixture.SetupHostAndInfra(services =>
        {
            services.AddMemoryCache(); 
            
            services.AddHttpClient<IAddressService, ViaCepAddressService>(client =>
            {
                client.BaseAddress = new Uri(_wireMockFixture.MockHttpContainer.GetPublicUrl());
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
                
            services.AddHttpClient<IEmailService<BrevoEmailRequest>, BrevoEmailService>(client =>
            {
                client.BaseAddress = new Uri(_wireMockFixture.MockHttpContainer.GetPublicUrl());
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("api-key", _sharedFixture.Configuration["Brevo:ApiKey"]);
            });
                
            services.AddHostedService<UpdateBloodStockOnDonationSubscriber>();
            services.AddHostedService<SendEmailOnDonationSubscriber>();
            
        });
        
        _wireMockClient = _wireMockFixture.client;
        
        await using var scope = _sharedFixture.ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();
        
        await dbContext.Donations.ExecuteDeleteAsync();
        await dbContext.BloodDonors.ExecuteDeleteAsync();
        await dbContext.Addresses.ExecuteDeleteAsync();
    }
    
    [Fact]
    public async Task TwoDonorsSameAddress_ShouldCacheZipCodeAndShareAddress()
    {
        await using var scope = _sharedFixture.ServiceProvider.CreateAsyncScope();
        
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var bloodDonorsRepository = scope.ServiceProvider.GetRequiredService<IBloodDonorsRepository>();
        var addressService = scope.ServiceProvider.GetRequiredService<IAddressService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();
        
        var handler = new RegisterDonorHandler(addressService, bloodDonorsRepository, unitOfWork);
        var firstDonorCommand = RegisterDonorCommandFaker.Generate(true);
        var secondDonorCommand = RegisterDonorCommandFaker.Generate(true);
        secondDonorCommand.Zipcode = firstDonorCommand.Zipcode;
        secondDonorCommand.Number = firstDonorCommand.Number;
        secondDonorCommand.Complement = firstDonorCommand.Complement;
        
        var viaCepResponse = FakeZipcodeDatabase.GetResponse(firstDonorCommand.Zipcode);
        

        await ReturnWireMockMapping("GET", $"/{firstDonorCommand.Zipcode}/json/", 200, viaCepResponse);
        
        await handler.Handle(firstDonorCommand, CancellationToken.None);
        await handler.Handle(secondDonorCommand, CancellationToken.None);
        
        var requests = await _wireMockClient.FindRequestsAsync(new RequestModel
        {
            Path = $"/{firstDonorCommand.Zipcode}/json/",
            Methods = ["GET"]
        });
        
        Assert.Single(requests);
        Assert.Equal(2, dbContext.BloodDonors.Count());
        
        var donor1 = await dbContext.BloodDonors.Include(d => d.Address).FirstAsync(d => d.Email == firstDonorCommand.Email);
        var donor2 = await dbContext.BloodDonors.Include(d => d.Address).FirstAsync(d => d.Email == secondDonorCommand.Email);

        Assert.Equal(donor1.Address.Id, donor2.Address.Id);
        Assert.Single(dbContext.Addresses);
    }
    
     [Fact]
    public async Task DonationRegistered_EventIsPublishedToRabbitMq_BloodStockWasUpdatedAndThankYouEmailSent()
    {
        await using var scope = _sharedFixture.ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloodBankDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var bloodDonorRepository = scope.ServiceProvider.GetRequiredService<IBloodDonorsRepository>();
        var donationsRepository = scope.ServiceProvider.GetRequiredService<IDonationRepository>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var donor = BloodDonorFaker.Generate();
        
        await dbContext.Addresses.AddAsync(donor.Address);
        await dbContext.SaveChangesAsync();
        await dbContext.BloodDonors.AddAsync(donor);
        
        var bloodStock = new BloodStock(donor.BloodType, 1000);
        await dbContext.BloodStocks.AddAsync(bloodStock);
        await dbContext.SaveChangesAsync();
        
        var initialStock = bloodStock.QuantityInMl;
     
        var headers = new Dictionary<string, string>
        {
            ["api-key"] = _sharedFixture.Configuration["Brevo:ApiKey"]
        };
        
        await ReturnWireMockMapping("POST",  BrevoUri, 200, headers);

        
        var handler = new RegisterDonationHandler(unitOfWork, bloodDonorRepository,  donationsRepository, outboxRepository);
        var command = RegisterDonationCommandFaker.Generate(donor.Id);
        var donationAmount = command.QuantityInMl;
        
        
        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        
        var outboxMessage = await dbContext.OutboxMessages
            .FirstOrDefaultAsync(m => m.EventType.Contains("DonationRegistered"));
        
        Assert.NotNull(outboxMessage);
        Assert.Equal(OutboxMessageStatus.Pending.ToString(), outboxMessage.Status.ToString());

        var expirationTimeout = TimeSpan.FromSeconds(60);
       
        var stockUpdated = await WaitForConditionAsync(async () => 
        {
            await dbContext.Entry(bloodStock).ReloadAsync();
            return bloodStock.QuantityInMl == initialStock + donationAmount;
        }, expirationTimeout);
        
        Assert.True(stockUpdated, "The blood stock was not updated within the expected time");
        
        var outboxProcessed = await WaitForConditionAsync(async () => 
        {
            await dbContext.Entry(outboxMessage).ReloadAsync();
            return outboxMessage.Status == OutboxMessageStatus.Processed;
        }, expirationTimeout);
        
        Assert.True(outboxProcessed, "The outbox message was not processed in the expected time");

        var emailSent = await WaitForConditionAsync(async () =>
        {
            var requests = await _wireMockClient.FindRequestsAsync(new RequestModel
            {
                Path = BrevoUri,
                Methods = ["POST"]
            });
            
            return requests.Count == 1;
        }, expirationTimeout);
        
        Assert.True(emailSent, "The email was not sent in the expected time");
    }

    private async Task ReturnWireMockMapping(string httpMethod, string path, int statusCode,  object? content = null,  IDictionary<string, string>? requestHeaders = null)
    { 
        
        var response = new ResponseModel
        {
            StatusCode = statusCode
        };
        
        if (content != null)
        {
            response.BodyAsJson = content;
            response.Headers = new Dictionary<string, object>
            {
                ["Content-Type"] = "application/json"
            };
        }


        var request = new RequestModel
        {
            Path = path,
            Methods = [httpMethod]
        };
        
        if (requestHeaders != null && requestHeaders.Any())
        {
            request.Headers = requestHeaders
                .Select(kv => new HeaderModel
                {
                    Name     = kv.Key,
                    Matchers = new List<MatcherModel>
                    {
                        new MatcherModel
                        {
                            Name    = "ExactMatcher",      
                            Pattern = kv.Value
                        }
                    }
                })
                .ToList();
        }

        await _wireMockClient.PostMappingAsync(new MappingModel
        { 
            Request = request,
            Response = response
        });
    }
    

    public async Task DisposeAsync()
    {

    }
}
