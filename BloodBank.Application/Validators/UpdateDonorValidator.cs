using BloodBank.Application.Commands.Donors.UpdateDonor;
using FluentValidation;

namespace BloodBank.Application.Validators;

public class UpdateDonorValidator : AbstractValidator<UpdateDonorCommand>
{
    public UpdateDonorValidator()
    {
        
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .Matches(@"^[\p{L}\s']+$").WithMessage("Name contains invalid characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Weight)
            .NotEmpty().WithMessage("Weight is required");

        RuleFor(x => x.Zipcode)
            .NotEmpty().WithMessage("Zipcode is required")
            .Matches(@"^\d{8}$").WithMessage("Zipcode must contain exactly 8 digits")
            .Must(ValidatorUtils.BeAllDigits).WithMessage("Zipcode must contain only numbers");
    }
}