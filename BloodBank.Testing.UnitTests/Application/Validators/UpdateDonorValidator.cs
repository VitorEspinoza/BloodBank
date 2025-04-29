using BloodBank.Application.Commands.Donors.UpdateDonor;
using BloodBank.Application.Validators;
using FluentValidation.TestHelper;

namespace BloodBank.Testing.UnitTests.Application.Validators;


public class UpdateDonorValidatorTests
{
    private readonly UpdateDonorValidator _validator = new UpdateDonorValidator();
    
    [Theory]
    [InlineData("João Silva")]
    [InlineData("Maria das Dores")]
    public void FullName_Valid_ShouldNotHaveError(string name)
    {
        var command = new UpdateDonorCommand { FullName = name };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.FullName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")] 
    [InlineData("João2 Silva")] 
    public void FullName_Invalid_ShouldHaveError(string name)
    {
        var command = new UpdateDonorCommand { FullName = name };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }
    
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@domain.co")]
    public void Email_Valid_ShouldNotHaveError(string email)
    {
        var command = new UpdateDonorCommand { Email = email };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")] 
    [InlineData("invalid-email")] 
    public void Email_Invalid_ShouldHaveError(string email)
    {
        var command = new UpdateDonorCommand { Email = email };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
    
    [Fact]
    public void Weight_Valid_ShouldNotHaveError()
    {
        var command = new UpdateDonorCommand { Weight = 70.5 };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Weight);
    }

    [Fact]
    public void Weight_Invalid_ShouldHaveError()
    {
        var command = new UpdateDonorCommand { Weight = 0 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Weight);
    }
    
    [Theory]
    [InlineData("12345678")]
    public void Zipcode_Valid_ShouldNotHaveError(string zipcode)
    {
        var command = new UpdateDonorCommand { Zipcode = zipcode };
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
        var command = new UpdateDonorCommand { Zipcode = zipcode };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Zipcode);
    }
}