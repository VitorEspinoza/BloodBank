
namespace BloodBank.Core.Entities;

public class Address : BaseEntity
{
    public Address(string street, string neighborhood, string city, string state, string zipCode, string number, string? complement = null)
    {
        Street = street;
        Neighborhood =  neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
        Number = number;
        Complement = complement;
    }

    public string Street { get; private set; }
    public string Neighborhood { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string Number { get; private set; }
    public string? Complement { get; private set; }
}