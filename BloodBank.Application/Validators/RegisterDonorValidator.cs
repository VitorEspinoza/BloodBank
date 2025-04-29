using BloodBank.Application.Commands.Donors.RegisterDonor;
using BloodBank.Core.Enums;
using FluentValidation;

namespace BloodBank.Application.Validators;

public class RegisterDonorValidator: AbstractValidator<RegisterDonorCommand>
{
    public RegisterDonorValidator()
    {
      
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .Matches(@"^[\p{L}\s']+$").WithMessage("Name contains invalid characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date is required");

            RuleFor(x => x.BiologicalSex)
                .NotEmpty().WithMessage("Biological sex is required")
                .Must(ValidatorUtils.BeValidText).WithMessage("Invalid biological sex")
                .Must(s => Enum.TryParse<BiologicalSex>(s, true, out _))
                .WithMessage("Biological sex must be: Male or Female");

            RuleFor(x => x.Weight)
                .NotEmpty().WithMessage("Weight is required");

            RuleFor(x => x.BloodTypeGroup)
                .NotEmpty().WithMessage("Blood type is required")
                .Must(ValidatorUtils.BeValidText).WithMessage("Invalid blood type format")
                .Must(s => Enum.TryParse<BloodTypeGroup>(s, true, out _))
                .WithMessage("Valid values: A, B, AB, O");

            RuleFor(x => x.RhFactor)
                .NotEmpty().WithMessage("RH factor must be: Positive or Negative")
                .Must(ValidatorUtils.BeValidText).WithMessage("Invalid Rh factor")
                .Must(s => Enum.TryParse<RhFactor>(s, true, out _))
                .WithMessage("Invalid RH factor");

            RuleFor(x => x.Zipcode)
                .NotEmpty().WithMessage("Zipcode is required")
                .Matches(@"^\d{8}$").WithMessage("Zipcode must contain exactly 8 digits")
                .Must(ValidatorUtils.BeAllDigits).WithMessage("Zipcode must contain only numbers");
    }
    
}