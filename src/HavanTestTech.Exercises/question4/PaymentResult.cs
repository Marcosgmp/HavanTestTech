namespace HavanTestTech.Exercises.Question4;

public sealed record PaymentResult(
    DateOnly PaymentDate,
    decimal BaseAmount,
    int DaysEarly = 0,
    int DaysLate = 0,
    decimal Discount = 0m,
    decimal Fine = 0m,
    decimal Interest = 0m)
{
    public decimal FinalAmount => BaseAmount - Discount + Fine + Interest;
}