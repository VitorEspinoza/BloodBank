using BloodBank.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BloodBank.Core;

public static class CoreModule
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services
            .AddDomainServices();
            
        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<DonorEligibilityService>();
        return services;
    }
}