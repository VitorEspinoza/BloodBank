namespace BloodBank.Application.Validators;

public static class ValidatorUtils
{
    public static bool BeAllDigits(string text)
    {
        return BeValidText(text) && 
               text.All(char.IsDigit);
    }
    
    public static bool BeValidText(string text)
    {
        return !string.IsNullOrWhiteSpace(text);
    }
}