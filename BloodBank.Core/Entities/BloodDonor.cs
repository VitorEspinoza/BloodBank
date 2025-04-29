using BloodBank.Core.Enums;
using BloodBank.Core.ValueObjects;

namespace BloodBank.Core.Entities;

public class BloodDonor : BaseEntity
{
    private BloodDonor() { }
    public BloodDonor(string fullName, string email, DateTime birthDate, BiologicalSex biologicalSex, double weight, BloodType bloodType, Address address)
    {
        FullName = fullName;
        Email = email;
        BirthDate = birthDate;
        BiologicalSex = biologicalSex;
        Weight = weight;
        BloodType = bloodType;
        Donations = [];
        Address = address;
    }

    public string FullName { get; private set; }
    public string Email { get; private set; }
    public DateTime BirthDate { get; private set; }
    public BiologicalSex BiologicalSex { get; private set; }
    public double Weight { get; private set; }
    public BloodType BloodType { get; private set; }
    public List<Donation> Donations { get; private set; }
    public int AddressId { get; private set; }  
    public Address Address { get; private set; }
    
    public void Update(string fullName, string email, DateTime birthDate, BiologicalSex biologicalSex, double weight, BloodType bloodType)
    {
        FullName = fullName;
        Email = email;
        BirthDate = birthDate;
        BiologicalSex= biologicalSex;
        Weight = weight;
        BloodType = bloodType;
    }

    public void UpdateAddress(Address address)
         {
             Address = address;
         }
}
