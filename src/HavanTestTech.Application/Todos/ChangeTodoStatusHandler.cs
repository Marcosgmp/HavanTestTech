using HavanTestTech.Application.Abstractions;
using HavanTestTech.Application.Exceptions;
using HavanTestTech.Domain.Enums;

namespace HavanTestTech.Application.Todos;

public sealed class ChangeTodoStatusHandler(ITodoRepository repository, TimeProvider timeProvider)
{
    public async Task<TodoResponse> HandleAsync(
        Guid id,
        TodoStatus newStatus,
        CancellationToken cancellationToken)
    {
        // JSON binding accepts any integer for an enum, so out-of-range values are rejected here.
        if (!Enum.IsDefined(newStatus))
        {
            throw new InvalidRequestException("The provided status is not valid.");
        }

        var item = await repository.GetByIdAsync(id, cancellationToken)
                   ?? throw new TodoNotFoundException(id);

        item.ChangeStatus(newStatus, timeProvider.GetUtcNow().UtcDateTime);
        await repository.UpdateAsync(item, cancellationToken);

        return TodoResponse.From(item);
    }
}