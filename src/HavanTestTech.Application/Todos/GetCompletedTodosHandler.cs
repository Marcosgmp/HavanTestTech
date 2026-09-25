using HavanTestTech.Application.Abstractions;
using HavanTestTech.Application.Exceptions;

namespace HavanTestTech.Application.Todos;

public sealed class GetCompletedTodosHandler(ITodoRepository repository)
{
    public async Task<IReadOnlyList<TodoResponse>> HandleAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        if (startDate > endDate)
        {
            throw new InvalidRequestException("The start date cannot be after the end date.");
        }

        // Completion timestamps are stored in UTC, so the requested days are read as UTC days.
        // The end date is inclusive, which makes the upper bound the start of the following day.
        var rangeStart = startDate.ToDateTime(TimeOnly.MinValue);
        var rangeEnd = endDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var items = await repository.GetCompletedBetweenAsync(rangeStart, rangeEnd, cancellationToken);
        return items.Select(TodoResponse.From).ToList();
    }
}