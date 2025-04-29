using BloodBank.Application.Commands.Donations.RegisterDonation;
using FluentValidation;

namespace BloodBank.Application.Validators;

public class DonationValidator : AbstractValidator<RegisterDonationCommand>
{

    private const int MinQuantity = 420;
    private const int MaxQuantity = 470;

    public DonationValidator()
    {
        RuleFor(x => x.QuantityInMl)
            .InclusiveBetween(MinQuantity, MaxQuantity)
            .WithMessage($"Quantity must be {MinQuantity}-{MaxQuantity}ml");

        RuleFor(x => x.BloodDonorId)
            .GreaterThan(0)
            .WithMessage("Invalid Donor ID");
      
    }
}