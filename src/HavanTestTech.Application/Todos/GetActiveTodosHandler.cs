using HavanTestTech.Application.Abstractions;

namespace HavanTestTech.Application.Todos;

public sealed class GetActiveTodosHandler(ITodoRepository repository)
{
    public async Task<IReadOnlyList<TodoResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var items = await repository.GetActiveAsync(cancellationToken);
        return items.Select(TodoResponse.From).ToList();
    }
}