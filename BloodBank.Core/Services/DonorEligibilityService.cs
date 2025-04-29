using BloodBank.Core.Enums;
using BloodBank.Core.Models;
using BloodBank.Core.Repositories;

namespace BloodBank.Core.Services;

public class DonorEligibilityService
{
    private const int MinWeight = 50;
    private const int MinAge = 18;
    private const int MinIntervalMale = 60;
    private const int MinIntervalFemale = 90;
    
    public EligibilityResult CheckEligibility(DonorEligibilityData data)
    {
        var reasons = new List<string>();
        
        if (!data.DonorExists)
            return EligibilityResult.NotEligible("Donor not found");
            
        if (data.Age < MinAge)
            reasons.Add($"Minimum age: {MinAge} years");
            
        if (data.Weight < MinWeight)
            reasons.Add($"Minimum weight: {MinWeight}kg");

        var minDays = data.BiologicalSex == BiologicalSex.Male 
            ? MinIntervalMale 
            : MinIntervalFemale;
                  
        if (data.DaysSinceLastDonation < minDays && data.DaysSinceLastDonation != null)
            reasons.Add($"Wait {minDays} days between donations");
            
        return reasons.Count > 0
            ? EligibilityResult.NotEligible(reasons) 
            : EligibilityResult.Eligible();
    }
    
}