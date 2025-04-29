using BloodBank.Application.Commands.Donations.RegisterDonation;
using Bogus;

namespace BloodBank.Testing.Common.Fakers;

public class RegisterDonationCommandFaker
{
    public static RegisterDonationCommand Generate(int? bloodDonorId = null, int? quantityInMl = null)
    {
        var faker = new Faker("pt_BR");

        return new RegisterDonationCommand
        {
            BloodDonorId = bloodDonorId ?? faker.Random.Int(1, 9999),
            QuantityInMl = quantityInMl ?? faker.Random.Int(420, 470) 
        };
    }
}