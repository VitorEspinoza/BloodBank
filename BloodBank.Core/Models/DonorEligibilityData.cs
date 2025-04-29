using BloodBank.Core.Enums;

namespace BloodBank.Core.Models;

public class DonorEligibilityData
{
    public DonorEligibilityData(bool donorExists, int age, double weight, int? daysSinceLastDonation, BiologicalSex biologicalSex)
    {
        DonorExists = donorExists;
        Age = age;
        Weight = weight;
        DaysSinceLastDonation = daysSinceLastDonation;
        BiologicalSex = biologicalSex;
    }
    public DonorEligibilityData(bool donorExists)
    {
        DonorExists = donorExists;
    }

    public bool DonorExists { get; }
    public int? Age { get; }
    public double? Weight { get; }
    public int? DaysSinceLastDonation { get; }
    
    public BiologicalSex BiologicalSex  { get; }
}