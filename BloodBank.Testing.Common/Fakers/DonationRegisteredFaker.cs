using BloodBank.Core.DomainEvents.Donations;
using Bogus;

namespace BloodBank.Testing.Common.Fakers;

public static class DonationRegisteredFaker
{
    private static readonly Faker Faker = new("pt_BR");

    public static DonationRegistered Generate(string? nameIdentifier = null)
    {
        return new DonationRegistered(
            DonationId: Faker.Random.Int(1, 100),
            BloodDonorId: Faker.Random.Int(1, 100),
            BloodType: BloodTypeFaker.Generate(),
            QuantityMl: Faker.Random.Int(420, 470),
            BloodDonorEmail: Faker.Person.Email,
            BloodDonorName: nameIdentifier ?? Faker.Person.FullName,
            DateTime.Now
        );
    }
}