using BloodBank.Core.Entities;

namespace BloodBank.Application.ViewModels;

public class DonationViewModel
{
    public DonationViewModel(int id, DateTime donationDate, int quantityInMl, BloodDonorSimpleViewModel bloodDonor)
    {
        Id = id;
        DonationDate = donationDate;
        QuantityInMl = quantityInMl;
        BloodDonor = bloodDonor;
    }

    public int Id { get;  set; }
    public DateTime DonationDate { get;  set; }
    public int QuantityInMl { get;  set; }
    public BloodDonorSimpleViewModel BloodDonor { get;  set; }
    
    public static DonationViewModel fromEntity(Donation entity) => new (
        entity.Id,
        entity.DonationDate,
        entity.QuantityInMl, 
        new BloodDonorSimpleViewModel(
            entity.BloodDonor.Id,
            entity.BloodDonor.FullName)
    );
}