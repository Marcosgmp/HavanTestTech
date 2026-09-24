namespace HavanTestTech.Domain.Exceptions;

/// <summary>
/// Signals a violation of a business rule.
/// </summary>
public sealed class DomainException(string message) : Exception(message);