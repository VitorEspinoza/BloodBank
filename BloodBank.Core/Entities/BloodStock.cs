using BloodBank.Core.Enums;
using BloodBank.Core.ValueObjects;

namespace BloodBank.Core.Entities;

public class BloodStock : BaseEntity
{
    private BloodStock() { }
    
    public BloodStock(BloodType bloodType, int quantityInMl)
    {
        BloodType = bloodType;
        QuantityInMl = quantityInMl;
    }
    public BloodType BloodType { get; private set; }
    public int QuantityInMl { get; private set; }
    
    public void AddQuantityInMl(int quantity)
    {
        QuantityInMl += quantity;
    }
    
    public static BloodStock CreateInitial(BloodType bloodType)
    {
        return new BloodStock(bloodType, 0);
    }
    
}