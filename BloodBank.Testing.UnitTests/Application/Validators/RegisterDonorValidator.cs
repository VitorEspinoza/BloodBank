using BloodBank.Application.Commands.Donors.RegisterDonor;
using BloodBank.Application.Validators;
using FluentValidation.TestHelper;

namespace BloodBank.Testing.UnitTests.Application.Validators;

public class RegisterDonorValidatorTests
{
    private readonly RegisterDonorValidator _validator = new RegisterDonorValidator();
    
    [Theory]
    [InlineData("João Silva")]
    [InlineData("Maria das Dores")]
    public void FullName_Valid_ShouldNotHaveError(string name)
    {
        var command = new RegisterDonorCommand { FullName = name };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.FullName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")] 
    [InlineData("João2 Silva")] 
    public void FullName_Invalid_ShouldHaveError(string name)
    {
        var command = new RegisterDonorCommand { FullName = name };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }
    
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@domain.co")]
    public void Email_Valid_ShouldNotHaveError(string email)
    {
        var command = new RegisterDonorCommand { Email = email };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")] 
    public void Email_Invalid_ShouldHaveError(string email)
    {
        var command = new RegisterDonorCommand { Email = email };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
    
    [Fact]
    public void BirthDate_Valid_ShouldNotHaveError()
    {
        var command = new RegisterDonorCommand { BirthDate = new DateTime(1990, 1, 1) };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.BirthDate);
    }

    [Fact]
    public void BirthDate_Invalid_ShouldHaveError()
    {
        var command = new RegisterDonorCommand { BirthDate = default };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BirthDate);
    }
    
    [Theory]
    [InlineData("Male")]
    [InlineData("Female")]
    [InlineData("male")]
    public void BiologicalSex_Valid_ShouldNotHaveError(string sex)
    {
        var command = new RegisterDonorCommand { BiologicalSex = sex };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.BiologicalSex);
    }

    [Theory]
    [InlineData("")] 
    [InlineData("Invalid")] 
    public void BiologicalSex_Invalid_ShouldHaveError(string sex)
    {
        var command = new RegisterDonorCommand { BiologicalSex = sex };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BiologicalSex);
    }
    
    [Fact]
    public void Weight_Valid_ShouldNotHaveError()
    {
        var command = new RegisterDonorCommand { Weight = 70.5 };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Weight);
    }

    [Fact]
    public void Weight_Invalid_ShouldHaveError()
    {
        var command = new RegisterDonorCommand { Weight = 0 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Weight);
    }
    
    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    [InlineData("AB")]
    [InlineData("O")]
    public void BloodTypeGroup_Valid_ShouldNotHaveError(string bloodType)
    {
        var command = new RegisterDonorCommand { BloodTypeGroup = bloodType };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.BloodTypeGroup);
    }

    [Theory]
    [InlineData("")] 
    [InlineData("X")] 
    public void BloodTypeGroup_Invalid_ShouldHaveError(string bloodType)
    {
        var command = new RegisterDonorCommand { BloodTypeGroup = bloodType };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BloodTypeGroup);
    }
    
    [Theory]
    [InlineData("Positive")]
    [InlineData("Negative")]
    public void RhFactor_Valid_ShouldNotHaveError(string rhFactor)
    {
        var command = new RegisterDonorCommand { RhFactor = rhFactor };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.RhFactor);
    }

    [Theory]
    [InlineData("")] 
    [InlineData("Invalid")] 
    public void RhFactor_Invalid_ShouldHaveError(string rhFactor)
    {
        var command = new RegisterDonorCommand { RhFactor = rhFactor };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RhFactor);
    }
    
    [Theory]
    [InlineData("12345678")]
    public void Zipcode_Valid_ShouldNotHaveError(string zipcode)
    {
        var command = new RegisterDonorCommand { Zipcode = zipcode };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Zipcode);
    }

    [Theory]
    [InlineData("")] 
    [InlineData("1234567")] 
    [InlineData("123456789")] 
    [InlineData("1234A678")] 
    public void Zipcode_Invalid_ShouldHaveError(string zipcode)
    {
        var command = new RegisterDonorCommand { Zipcode = zipcode };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Zipcode);
    }
}