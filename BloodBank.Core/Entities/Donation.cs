using BloodBank.Core.DomainEvents;
using BloodBank.Core.DomainEvents.Donations;
using BloodBank.Core.ValueObjects;

namespace BloodBank.Core.Entities;

public class Donation : BaseEntity
{
    public Donation(BloodDonor bloodDonor, int quantityInMl)
    {
        BloodDonorId = bloodDonor.Id;
        BloodDonor = bloodDonor;
        DonationDate = DateTime.UtcNow;
        QuantityInMl = quantityInMl;
    }
    private Donation() { } 
    public int BloodDonorId { get; private set; }
    public DateTime DonationDate { get; private set; }
    public int QuantityInMl { get; private set; }
    public BloodDonor BloodDonor { get; private set; }

    public DonationRegistered RegisterDonationEvent(BloodType bloodType, string email, string name)
    {
        return new DonationRegistered(Id, BloodDonorId, bloodType, QuantityInMl, email, name, DonationDate);
    }
    
}