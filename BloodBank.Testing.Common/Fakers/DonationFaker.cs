using BloodBank.Core.Entities;
using Bogus;

namespace BloodBank.Testing.Common.Fakers;

public static class DonationFaker
{
    private static readonly Faker Faker = new("pt_BR");
    
    public static Donation Generate()
    {
        return new Donation(
            bloodDonor: BloodDonorFaker.Generate(),
            quantityInMl: Faker.Random.Int(420, 470)
        );
    }
    public static List<Donation> GenerateList(int quantity)
    {
        var donations = new List<Donation>();
    
        for (var i = 0; i < quantity; i++)
        {
            donations.Add(Generate());
        }
    
        return donations;
    }
    
    
}