using BloodBank.Core.Enums;
using BloodBank.Core.Models;
using BloodBank.Core.Services;
using Xunit;

namespace BloodBank.Testing.UnitTests.Core.Services;

public class DonorEligibilityServiceTests
{
    private readonly DonorEligibilityService _service = new();

    [Fact]
    public void DonorBelowMinimumAge_NotEligible_ReturnsMinimumAgeReason()
    {
        var data = new DonorEligibilityData(
            donorExists: true,
            age: 17,
            weight: 60,
            daysSinceLastDonation: null,
            biologicalSex: BiologicalSex.Male);

        var result = _service.CheckEligibility(data);
        
        Assert.False(result.IsEligible);
        Assert.Single(result.Reasons);
        Assert.Contains("Minimum age: 18 years", result.Reasons);
    }

    [Fact]
    public void DonorBelowMinimumWeight_NotEligible_ReturnsMinimumWeightReason()
    {
        var data = new DonorEligibilityData(
            donorExists: true,
            age: 20,
            weight: 49, 
            daysSinceLastDonation: null,
            biologicalSex: BiologicalSex.Female);

        var result = _service.CheckEligibility(data);
        
        Assert.False(result.IsEligible);
        Assert.Single(result.Reasons);
        Assert.Contains("Minimum weight: 50kg", result.Reasons);
    }

    [Fact]
    public void MaleDonorRecentDonation_NotEligible_ReturnsWaitDaysReason()
    {
        var data = new DonorEligibilityData(
            donorExists: true,
            age: 25,
            weight: 70,
            daysSinceLastDonation: 30, 
            biologicalSex: BiologicalSex.Male);

        var result = _service.CheckEligibility(data);
        
        Assert.False(result.IsEligible);
        Assert.Single(result.Reasons);
        Assert.Contains("Wait 60 days between donations", result.Reasons);
    }

    [Fact]
    public void FemaleDonorRecentDonation_NotEligible_ReturnsWaitDaysReason()
    {
        var data = new DonorEligibilityData(
            donorExists: true,
            age: 25,
            weight: 70,
            daysSinceLastDonation: 60,
            biologicalSex: BiologicalSex.Female);

        var result = _service.CheckEligibility(data);
        
        Assert.False(result.IsEligible);
        Assert.Single(result.Reasons);
        Assert.Contains("Wait 90 days between donations", result.Reasons);
    }

    [Fact]
    public void DonorWithMultipleReasons_NotEligible_ReturnsAllReasons()
    {
        var data = new DonorEligibilityData(
            donorExists: true,
            age: 17, 
            weight: 45,
            daysSinceLastDonation: 30,
            biologicalSex: BiologicalSex.Male);

        var result = _service.CheckEligibility(data);
        
        Assert.False(result.IsEligible);
        Assert.Equal(3, result.Reasons.Count);
        Assert.Contains("Minimum age: 18 years", result.Reasons);
        Assert.Contains("Minimum weight: 50kg", result.Reasons);
        Assert.Contains("Wait 60 days between donations", result.Reasons);
    }

    [Fact]
    public void EligibleMaleDonor_NoRecentDonation_ReturnsEligible()
    {
        var data = new DonorEligibilityData(
            donorExists: true,
            age: 25,
            weight: 70,
            daysSinceLastDonation: null, 
            biologicalSex: BiologicalSex.Male);

        var result = _service.CheckEligibility(data);
        
        Assert.True(result.IsEligible);
        Assert.Empty(result.Reasons);
    }

    [Fact]
    public void EligibleFemaleDonor_SufficientTimeSinceLastDonation_ReturnsEligible()
    {
        var data = new DonorEligibilityData(
            donorExists: true,
            age: 25,
            weight: 70,
            daysSinceLastDonation: 95,
            biologicalSex: BiologicalSex.Female);

        var result = _service.CheckEligibility(data);
        
        Assert.True(result.IsEligible);
        Assert.Empty(result.Reasons);
    }

    [Fact]
    public void EligibleMaleDonor_SufficientTimeSinceLastDonation_ReturnsEligible()
    {
        var data = new DonorEligibilityData(
            donorExists: true,
            age: 25,
            weight: 70,
            daysSinceLastDonation: 65,
            biologicalSex: BiologicalSex.Male);

        var result = _service.CheckEligibility(data);
        
        Assert.True(result.IsEligible);
        Assert.Empty(result.Reasons);
    }

    [Fact]
    public void EligibleExactMinimumValues_ReturnsEligible()
    {
        const int minAge = 18;
        const int minWeight = 50;
        const int minDaysSinceLastDonationForMale = 60;
        
        var data = new DonorEligibilityData(
            donorExists: true,
            age: minAge, 
            weight: minWeight, 
            daysSinceLastDonation: minDaysSinceLastDonationForMale, 
            biologicalSex: BiologicalSex.Male);

        var result = _service.CheckEligibility(data);
        
        Assert.True(result.IsEligible);
        Assert.Empty(result.Reasons);
    }
}