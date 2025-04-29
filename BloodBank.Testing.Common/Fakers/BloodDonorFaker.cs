using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using Bogus;

namespace BloodBank.Testing.Common.Fakers;

public static class BloodDonorFaker
{
    private static readonly Faker Faker = new("pt_BR");
    
    public static BloodDonor Generate()
    {
        return new BloodDonor(
            fullName: Faker.Name.FullName(),
            email: Faker.Internet.Email(),
            birthDate: Faker.Date.Past(30, DateTime.Today.AddYears(-18)),
            biologicalSex: Faker.PickRandom<BiologicalSex>(),
            weight: Faker.Random.Double(50, 120),
            bloodType: BloodTypeFaker.Generate(),
            address: GenerateAddress()
        );
    }
    
    public static List<BloodDonor> GenerateList(int quantity)
    {
        var donors = new List<BloodDonor>();
    
        for (var i = 0; i < quantity; i++)
        {
            donors.Add(Generate());
        }
    
        return donors;
    }
    public static Address GenerateAddress()
    {
        return new Address(
            street: Faker.Address.StreetName(),
            number: Faker.Random.Int(1, 9999).ToString(),
            complement: Faker.Address.SecondaryAddress(),
            city: Faker.Address.City(),
            state: Faker.Address.State(),
            zipCode: Faker.Address.ZipCode().Replace("-", ""),
            neighborhood: Faker.Address.StreetName()
        );
    }
    
   
    
}