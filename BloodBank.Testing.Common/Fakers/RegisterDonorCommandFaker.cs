using BloodBank.Application.Commands.Donors.RegisterDonor;
using BloodBank.Core.Enums;
using Bogus;

namespace BloodBank.Testing.Common.Fakers;

public static class RegisterDonorCommandFaker
{
    public static RegisterDonorCommand Generate(bool realAddress = false)
    {
        var faker = new Faker("pt_BR");
        
        return new RegisterDonorCommand
        {
            FullName = faker.Person.FullName,
            Email = faker.Person.Email,
            BirthDate = faker.Person.DateOfBirth,
            BiologicalSex = faker.Random.Bool() ? "Male" : "Female",
            Weight = faker.Random.Double(50, 120),
            BloodTypeGroup = faker.PickRandom<BloodTypeGroup>().ToString(),
            RhFactor = faker.PickRandom<RhFactor>().ToString(),
            Zipcode = realAddress ?  FakeZipcodeDatabase.GetRandomCep() : faker.Address.ZipCode(),
            Number = faker.Address.BuildingNumber(),
            Complement = faker.Address.SecondaryAddress()
        };
    }
}