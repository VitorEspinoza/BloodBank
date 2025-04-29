using BloodBank.Core.Entities;

namespace BloodBank.Application.ViewModels;

public class BloodStockViewModel
{
    public BloodStockViewModel(string bloodType, int quantityInMl)
    {
        BloodType = bloodType;
        QuantityInMl = quantityInMl;
    }
    public string BloodType { get; set; }
    public int QuantityInMl { get; set; }
    
    public static BloodStockViewModel FromEntity(BloodStock model) => new(  model.BloodType.ToString(), model.QuantityInMl);

}