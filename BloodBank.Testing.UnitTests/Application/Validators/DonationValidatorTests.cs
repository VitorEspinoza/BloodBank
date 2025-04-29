using BloodBank.Application.Commands.Donations.RegisterDonation;
using BloodBank.Application.Validators;
using FluentValidation.TestHelper;

namespace BloodBank.Testing.UnitTests.Application.Validators;

public class DonationValidatorTests
{
    private readonly DonationValidator _validator = new ();
    
    [Theory]
    [InlineData(420)]  
    [InlineData(445)]  
    [InlineData(470)]  
    public void QuantityInMl_ValidValues_ShouldNotHaveErrors(int quantity)
    {
        var command = new RegisterDonationCommand { QuantityInMl = quantity };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.QuantityInMl);
    }

    [Theory]
    [InlineData(419)]  
    [InlineData(471)]  
    public void QuantityInMl_InvalidValues_ShouldHaveErrors(int quantity)
    {
        var command = new RegisterDonationCommand { QuantityInMl = quantity };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.QuantityInMl)
            .WithErrorMessage("Quantity must be 420-470ml");
    }
    
    [Theory]
    [InlineData(1)]  
    [InlineData(1000)]
    public void BloodDonorId_ValidValues_ShouldNotHaveErrors(int donorId)
    {
        var command = new RegisterDonationCommand { BloodDonorId = donorId };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.BloodDonorId);
    }

    [Theory]
    [InlineData(0)]  
    [InlineData(-1)]  
    public void BloodDonorId_InvalidValues_ShouldHaveErrors(int donorId)
    {
        var command = new RegisterDonationCommand { BloodDonorId = donorId };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BloodDonorId)
            .WithErrorMessage("Invalid Donor ID");
    }
}