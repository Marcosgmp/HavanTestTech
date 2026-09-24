namespace HavanTestTech.Application.Exceptions;

/// <summary>
/// Signals input that is malformed or inconsistent, as opposed to a business rule violation.
/// </summary>
public sealed class InvalidRequestException(string message) : Exception(message);