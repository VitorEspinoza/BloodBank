using BloodBank.Core.Entities;

namespace BloodBank.Application.ViewModels;

public class AddressViewModel
{
    public AddressViewModel(string street, string neighborhood, string city, string state, string zipcode, string number, string complement)
    {
        Street = street;
        Neighborhood =  neighborhood;
        City = city;
        State = state;
        ZipCode = zipcode;
        Number = number;
        Complement = complement;
    }

    public string Street { get; private set; }
    public string Neighborhood { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string Number { get; private set; }
    public string Complement { get; private set; }
    
    public static AddressViewModel FromEntity(Address entity) => new(entity.Street,  entity.Neighborhood, entity.City,  entity.State, entity.ZipCode, entity.Number, entity.Complement);
}