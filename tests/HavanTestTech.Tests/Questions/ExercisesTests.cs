using System.Globalization;
using HavanTestTech.Exercises.Question2;
using HavanTestTech.Exercises.Question3;
using HavanTestTech.Exercises.Question4;

namespace HavanTestTech.Tests.Questions;

public class ConsecutiveSequenceFinderTests
{
    [Fact]
    public void FindLongest_WithUnorderedInput_ReturnsLongestRun()
    {
        var result = ConsecutiveSequenceFinder.FindLongest([100, 4, 200, 1, 3, 2]);

        Assert.Equal([1, 2, 3, 4], result);
    }

    [Fact]
    public void FindLongest_IgnoresDuplicatesAndSupportsNegatives()
    {
        var result = ConsecutiveSequenceFinder.FindLongest([-1, -2, -2, 0, 7]);

        Assert.Equal([-2, -1, 0], result);
    }

    [Fact]
    public void FindLongest_WithEmptyInput_ReturnsEmpty()
    {
        Assert.Empty(ConsecutiveSequenceFinder.FindLongest([]));
    }
}

public class PhraseAnalyzerTests
{
    [Fact]
    public void Sanitize_RemovesAccentsPunctuationAndCase()
    {
        Assert.Equal("acaocoracao", TextSanitizer.Sanitize("Ação, Coração!"));
    }

    [Fact]
    public void Analyze_WithChallengePhrase_ReturnsExpectedResult()
    {
        var analysis = PhraseAnalyzer.Analyze("A Bateria do computador está Fraca!");

        Assert.Equal("abateriadocomputadorestafraca", analysis.SanitizedText);
        Assert.Equal('b', analysis.FirstUniqueCharacter);
        Assert.Equal(
            [new CharacterFrequency('a', 7), new CharacterFrequency('t', 3), new CharacterFrequency('r', 3)],
            analysis.MostFrequentCharacters);
    }

    [Fact]
    public void Analyze_WhenEveryCharacterRepeats_HasNoUniqueCharacter()
    {
        Assert.Null(PhraseAnalyzer.Analyze("aabb").FirstUniqueCharacter);
    }
}

public class PaymentCalculatorTests
{
    private readonly PaymentCalculator _calculator = new(1000m, new DateOnly(2026, 10, 10));

    [Theory]
    [InlineData("2026-09-20", 900)]  // 20 days early: discount is capped at 10%
    [InlineData("2026-10-05", 950)]  // 5 days early: 5% discount
    [InlineData("2026-10-10", 1000)] // on the due date
    [InlineData("2026-10-15", 1045)] // 5 days late: 2% fine + 2.5% interest
    public void Calculate_ReturnsExpectedFinalAmount(string paymentDate, int expectedAmount)
    {
        var date = DateOnly.Parse(paymentDate, CultureInfo.InvariantCulture);

        var result = _calculator.Calculate(date);

        Assert.Equal((decimal)expectedAmount, result.FinalAmount);
    }

    [Fact]
    public void Calculate_WhenLate_BreaksDownFineAndInterest()
    {
        var result = _calculator.Calculate(new DateOnly(2026, 10, 15));

        Assert.Equal(20m, result.Fine);
        Assert.Equal(25m, result.Interest);
        Assert.Equal(0m, result.Discount);
    }
}
