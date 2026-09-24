namespace HavanTestTech.Application.Exceptions;

public sealed class TodoNotFoundException(Guid id)
    : Exception($"No task was found with id '{id}'.");