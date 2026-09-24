using System.Text;

namespace HavanTestTech.Exercises.Question3;

public static class TextSanitizer
{
    /// <summary>
    /// Lowercases the text, removes accents and keeps only letters and digits.
    /// </summary>
    public static string Sanitize(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        // Normalization form D splits "á" into "a" plus a combining accent mark.
        // Combining marks are not letters, so the filter below drops them and keeps the base letter.
        var decomposed = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }
}