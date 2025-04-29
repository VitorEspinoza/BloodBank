using BloodBank.Core.Enums;
using BloodBank.Core.ValueObjects;
using Bogus;

namespace BloodBank.Testing.Common.Fakers;

public static class BloodTypeFaker
{
    private static readonly Faker Faker = new("pt_BR");
    public static BloodType Generate()
    {
        var group = Faker.PickRandom<BloodTypeGroup>();
        var rh = Faker.PickRandom<RhFactor>();
        return new BloodType(group, rh);
    }
}