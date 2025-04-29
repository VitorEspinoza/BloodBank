using BloodBank.Core.Enums;
using static System.Enum;

namespace BloodBank.Core.ValueObjects;

public class BloodType
{
    private BloodType() {}
    public BloodTypeGroup Group { get; private set;  } 
    public RhFactor Rh { get; private set; }      
    
    public BloodType(BloodTypeGroup group, RhFactor rh)
    {
        Group = group;
        Rh = rh;
    }
    
    public static BloodType FromDatabase(string group, string rh)
    {
        if (!TryParse<BloodTypeGroup>(group, true, out var groupEnum))
            throw new InvalidOperationException($"Invalid blood group: {group}");
    
        if (!TryParse<RhFactor>(rh, true, out var rhEnum))
            throw new InvalidOperationException($"Invalid Rh factor: {rh}");

        return new BloodType(groupEnum, rhEnum);
    }

    public (string Group, string Rh) ToDatabase()
    {
        return (Group.ToString(), Rh.ToString());
    }
    
    public override string ToString() => $"{Group}{(Rh == RhFactor.Positive ? "+" : "-")}";
    public override bool Equals(object? obj)
    {
        return obj is BloodType other &&
               Group == other.Group &&
               Rh == other.Rh;
    }
    
    public override int GetHashCode() => HashCode.Combine(Group, Rh);
    
}

