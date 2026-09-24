namespace HavanTestTech.Exercises.Question3;

public static class PhraseAnalyzer
{
    private const int TopCharactersCount = 3;

    public static PhraseAnalysis Analyze(string phrase)
    {
        var sanitized = TextSanitizer.Sanitize(phrase);
        var frequencies = CountFrequencies(sanitized);

        return new PhraseAnalysis(
            sanitized,
            FindFirstUnique(sanitized, frequencies),
            FindMostFrequent(sanitized, frequencies));
    }

    private static Dictionary<char, int> CountFrequencies(string text)
    {
        var frequencies = new Dictionary<char, int>();

        foreach (var character in text)
        {
            frequencies[character] = frequencies.GetValueOrDefault(character) + 1;
        }

        return frequencies;
    }

    // Sanitizing preserves character order, so the first unique character of the
    // sanitized text is also the first one in the original phrase.
    private static char? FindFirstUnique(string text, Dictionary<char, int> frequencies)
    {
        foreach (var character in text)
        {
            if (frequencies[character] == 1)
            {
                return character;
            }
        }

        return null;
    }

    // Dictionary enumeration order is unspecified, so ties are broken by first appearance
    // in the text. This keeps the result deterministic.
    private static IReadOnlyList<CharacterFrequency> FindMostFrequent(
        string text,
        Dictionary<char, int> frequencies)
    {
        return frequencies
            .OrderByDescending(pair => pair.Value)
            .ThenBy(pair => text.IndexOf(pair.Key))
            .Take(TopCharactersCount)
            .Select(pair => new CharacterFrequency(pair.Key, pair.Value))
            .ToList();
    }
}