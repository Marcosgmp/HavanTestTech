namespace HavanTestTech.Exercises.Question4;

public sealed class PaymentCalculator(decimal baseAmount, DateOnly dueDate)
{
    private const decimal EarlyDiscountRatePerDay = 0.01m;
    private const decimal MaxDiscountRate = 0.10m;
    private const decimal LateFineRate = 0.02m;
    private const decimal LateInterestRatePerDay = 0.005m;

    public PaymentResult Calculate(DateOnly paymentDate)
    {
        var daysFromDueDate = paymentDate.DayNumber - dueDate.DayNumber;

        return daysFromDueDate switch
        {
            < 0 => CalculateEarlyPayment(paymentDate, daysEarly: -daysFromDueDate),
            > 0 => CalculateLatePayment(paymentDate, daysLate: daysFromDueDate),
            _ => new PaymentResult(paymentDate, baseAmount)
        };
    }

    private PaymentResult CalculateEarlyPayment(DateOnly paymentDate, int daysEarly)
    {
        var discountRate = Math.Min(daysEarly * EarlyDiscountRatePerDay, MaxDiscountRate);

        return new PaymentResult(
            paymentDate,
            baseAmount,
            DaysEarly: daysEarly,
            Discount: RoundToCents(baseAmount * discountRate));
    }

    // Interest is simple, not compound: it is always computed on the base amount,
    // never on the fine or on previously accrued interest.
    private PaymentResult CalculateLatePayment(DateOnly paymentDate, int daysLate)
    {
        return new PaymentResult(
            paymentDate,
            baseAmount,
            DaysLate: daysLate,
            Fine: RoundToCents(baseAmount * LateFineRate),
            Interest: RoundToCents(baseAmount * LateInterestRatePerDay * daysLate));
    }

    private static decimal RoundToCents(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}