using BloodBank.Core.Entities;
using Bogus;

namespace BloodBank.Testing.Common.Fakers;

public static class OutboxFaker
{
    private static readonly Faker Faker = new("pt_BR");

    public static OutboxMessage GenerateOutboxMessage()
    {
        return OutboxMessage.Create(
            Faker.Random.Word(),
            Faker.Random.String2(20),
            Faker.Random.Word(),
            Faker.Random.AlphaNumeric(10)
        );
    }
}