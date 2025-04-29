using System.Text;

namespace BloodBank.Infrastructure;

public static class Utils
{
    public static string ToDashCase(this string text)
    {
        if(text == null)
            throw new ArgumentNullException(nameof(text));
        
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));

        for (int i = 1; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsUpper(c)) {
                sb.Append('-');
                sb.Append(char.ToLowerInvariant(c));
            }
            else 
                sb.Append(c);
        }
        
        return sb.ToString();
    }
}