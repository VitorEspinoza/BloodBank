namespace BloodBank.Core.Models;

public class EligibilityResult
{
    public bool IsEligible => !Reasons.Any();
    
    public IReadOnlyList<string> Reasons { get; }
    
    private EligibilityResult(List<string> reasons)
    {
        Reasons = reasons.AsReadOnly();
    }
    
    public static EligibilityResult Eligible() => new EligibilityResult(new List<string>());
    
    public static EligibilityResult NotEligible(string reason) => 
        new EligibilityResult(new List<string> { reason });
    
    public static EligibilityResult NotEligible(List<string> reasons) => 
        new EligibilityResult(reasons);
    
    public EligibilityResult AddReason(string reason)
    {
        var newReasons = new List<string>(Reasons) { reason };
        return new EligibilityResult(newReasons);
    }
}