using BloodBank.Core.Entities;
using BloodBank.Testing.Common.Fakers;

namespace BloodBank.Testing.UnitTests.Core.Entities;

public class BloodStockTests
{
    [Fact]
    public void InitialQuantity_AddQuantityInMlIsCalled_QuantityIsIncreased()
    {
        var bloodType = BloodTypeFaker.Generate();
        var bloodStock = new BloodStock(bloodType, 100);

        bloodStock.AddQuantityInMl(200);
        
        Assert.Equal(300, bloodStock.QuantityInMl);
    }
}