using BloodBank.Application.Commands.Donors.UpdateDonor;
using Bogus;

namespace BloodBank.Testing.Common.Fakers;

public static class UpdateDonorCommandFaker
{
    public static UpdateDonorCommand Generate()
    {
        var faker = new Faker("pt_BR");
        
        return new UpdateDonorCommand
        {
            FullName = faker.Person.FullName,
            Email = faker.Person.Email,
            Weight = faker.Random.Double(50, 120),
            Zipcode = faker.Address.ZipCode(),
            Number = faker.Address.BuildingNumber(),
            Complement = faker.Address.SecondaryAddress()
        };
    }
}