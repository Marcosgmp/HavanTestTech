// src/HavanTestTech.Exercises/Program.cs
using System.Globalization;
using System.Text;
using HavanTestTech.Exercises.Question2;
using HavanTestTech.Exercises.Question3;
using HavanTestTech.Exercises.Question4;

namespace HavanTestTech.Exercises;

public static class Program
{
    public static void Main()
    {
        // The pt-BR culture makes currency and percentages print as "R$ 1.000,00" and "10,0%".
        CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
        Console.OutputEncoding = Encoding.UTF8;

        RunQuestion2();
        RunQuestion3();
        RunQuestion4();
    }

    private static void RunQuestion2()
    {
        Console.WriteLine("=== Questão 2 ===");

        int[] input = [100, 5, 200, 1, 3, 2];
        var sequence = ConsecutiveSequenceFinder.FindLongest(input);

        Console.WriteLine($"Entrada: [{string.Join(", ", input)}]");
        Console.WriteLine($"Saída: [{string.Join(", ", sequence)}] (Tamanho {sequence.Count})");
        Console.WriteLine();
    }

    private static void RunQuestion3()
    {
        Console.WriteLine("=== Questão 3 ===");

        const string phrase = "A Bateria do computador está Fraca!";
        var analysis = PhraseAnalyzer.Analyze(phrase);

        Console.WriteLine($"Entrada: \"{phrase}\"");
        Console.WriteLine($"Texto higienizado: \"{analysis.SanitizedText}\"");
        Console.WriteLine(analysis.FirstUniqueCharacter is { } unique
            ? $"Primeiro caractere não repetido: '{unique}'"
            : "Primeiro caractere não repetido: nenhum");

        Console.WriteLine("Top 3 caracteres mais frequentes:");
        foreach (var item in analysis.MostFrequentCharacters)
        {
            Console.WriteLine($"  Letra '{item.Character}': {item.Count} vezes");
        }

        Console.WriteLine();
    }

    private static void RunQuestion4()
    {
        Console.WriteLine("=== Questão 4 ===");

        var calculator = new PaymentCalculator(baseAmount: 1000m, dueDate: new DateOnly(2026, 10, 10));
        DateOnly[] paymentDates =
        [
            new(2026, 9, 20),
            new(2026, 10, 5),
            new(2026, 10, 10),
            new(2026, 10, 15)
        ];

        foreach (var paymentDate in paymentDates)
        {
            PrintPayment(calculator.Calculate(paymentDate));
            Console.WriteLine();
        }
    }

    private static void PrintPayment(PaymentResult result)
    {
        Console.WriteLine($"Pagamento em {result.PaymentDate:dd/MM/yyyy}");
        Console.WriteLine($"  Valor base: {result.BaseAmount:C}");

        if (result.DaysEarly > 0)
        {
            Console.WriteLine($"  Desconto ({result.DaysEarly} dia(s) de antecipação): -{result.Discount:C}");
        }
        else if (result.DaysLate > 0)
        {
            Console.WriteLine($"  Multa fixa: +{result.Fine:C}");
            Console.WriteLine($"  Juros ({result.DaysLate} dia(s) de atraso): +{result.Interest:C}");
        }
        else
        {
            Console.WriteLine("  Pagamento na data: sem desconto, multa ou juros");
        }

        Console.WriteLine($"  Valor final: {result.FinalAmount:C}");
    }
}