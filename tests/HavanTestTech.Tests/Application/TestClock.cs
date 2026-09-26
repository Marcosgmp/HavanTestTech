namespace HavanTestTech.Tests.Application;

/// <summary>
/// Controllable clock that lets tests decide exactly when each operation happens.
/// </summary>
internal sealed class TestClock(DateTimeOffset start) : TimeProvider
{
    private DateTimeOffset _now = start;

    public override DateTimeOffset GetUtcNow() => _now;

    public void Advance(TimeSpan amount) => _now += amount;
}
