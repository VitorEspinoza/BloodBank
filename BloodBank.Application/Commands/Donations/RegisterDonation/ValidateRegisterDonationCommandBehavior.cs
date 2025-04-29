using BloodBank.Application.ViewModels;
using BloodBank.Core.Repositories;
using BloodBank.Core.Services;
using MediatR;

namespace BloodBank.Application.Commands.Donations.RegisterDonation;

public class ValidateRegisterDonationCommandBehavior : IPipelineBehavior<RegisterDonationCommand, ResultViewModel<int>>
{
    
    private readonly DonorEligibilityService _eligibilityService;
    private readonly IBloodDonorsRepository _bloodDonorsRepository;
    
    public ValidateRegisterDonationCommandBehavior(DonorEligibilityService eligibilityService, IBloodDonorsRepository bloodDonorsRepository)
    {
        _eligibilityService = eligibilityService;
        _bloodDonorsRepository = bloodDonorsRepository;
    }

    public async Task<ResultViewModel<int>> Handle(
        RegisterDonationCommand request, 
        RequestHandlerDelegate<ResultViewModel<int>> next, 
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        
        var eligibilityData = await _bloodDonorsRepository.GetEligibilityDataAsync(request.BloodDonorId);

        if (!eligibilityData.DonorExists)
        {
            errors.Add($"Donor with ID {request.BloodDonorId} not found");
            return ResultViewModel<int>.Error(errors);
        }
        
        var eligibilityResult = _eligibilityService.CheckEligibility(eligibilityData);

        if (!eligibilityResult.IsEligible)
        {
            errors.AddRange(eligibilityResult.Reasons);
            return ResultViewModel<int>.Error(errors);
        }
        
        return await next();
    }
}