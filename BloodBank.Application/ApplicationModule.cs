using BloodBank.Application.Commands.Donations.RegisterDonation;
using BloodBank.Application.Commands.Donors.RegisterDonor;
using BloodBank.Application.Commands.Donors.UpdateDonor;
using BloodBank.Application.Subscribers;
using BloodBank.Application.Validators;
using BloodBank.Application.ViewModels;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BloodBank.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddHandlers()
            .AddValidation()
            .AddSubscribers();
        
        return services;
    }
    
    private static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<RegisterDonationCommand>());

        services
            .AddTransient<IPipelineBehavior<UpdateDonorCommand, ResultViewModel>, ValidateUpdateDonorCommandBehavior>();
        
        services
            .AddTransient<IPipelineBehavior<RegisterDonationCommand, ResultViewModel<int>>, ValidateRegisterDonationCommandBehavior>();
        
        services
            .AddTransient<IPipelineBehavior<RegisterDonorCommand, ResultViewModel<int>>, ValidateRegisterDonorCommandBehavior>();
        
        
        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<DonationValidator>();
        
        return services;
    }
    
    private static IServiceCollection AddSubscribers(this IServiceCollection services)
    {
        services.AddHostedService<UpdateBloodStockOnDonationSubscriber>();
        services.AddHostedService<SendEmailOnDonationSubscriber>();
        return services;
    }
    
}