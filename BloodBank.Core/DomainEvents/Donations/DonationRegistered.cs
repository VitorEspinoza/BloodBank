using BloodBank.Core.ValueObjects;

namespace BloodBank.Core.DomainEvents.Donations;

public record DonationRegistered(
    int DonationId,
    int BloodDonorId,
    BloodType BloodType,
    int QuantityMl,
    string BloodDonorEmail,
    string BloodDonorName,
    DateTime TriggeredAt
        ) : IDomainEvent;
    