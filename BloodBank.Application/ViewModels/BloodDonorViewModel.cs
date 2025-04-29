using BloodBank.Core.Entities;

namespace BloodBank.Application.ViewModels;

public class BloodDonorViewModel
{
    public BloodDonorViewModel(int id, string fullName, string email, DateTime birthDate, string biologicalSex, double weight, string bloodType, List<DonationSimpleViewModel>? donations, AddressViewModel address)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        BirthDate = birthDate;
        BiologicalSex = biologicalSex;
        Weight = weight;
        BloodType = bloodType;
        Donations = donations;
        Address = address;
    }

    public int Id { get; set; }
    public string FullName { get;  set; }
    public string Email { get;  set; }
    public DateTime BirthDate { get;  set; }
    public string BiologicalSex { get;  set; }
    public double Weight { get;  set; }
    public string BloodType { get;  set; }
    public List<DonationSimpleViewModel> Donations { get;  set; }
    public AddressViewModel Address { get;  set; }
    
    public static BloodDonorViewModel fromEntity(BloodDonor entity) => new (
        entity.Id,
        entity.FullName, 
        entity.Email, 
        entity.BirthDate, 
        entity.BiologicalSex.ToString(), 
        entity.Weight,  
        entity.BloodType.ToString(), 
        entity.Donations?.Select(d => new DonationSimpleViewModel(d.Id, d.DonationDate, d.QuantityInMl)).ToList(),
        entity.Address != null ? AddressViewModel.FromEntity(entity.Address) : null);
}