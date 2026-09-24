namespace HavanTestTech.Exercises.Question3;

public sealed record CharacterFrequency(char Character, int Count);

public sealed record PhraseAnalysis(
    string SanitizedText,
    char? FirstUniqueCharacter,
    IReadOnlyList<CharacterFrequency> MostFrequentCharacters);