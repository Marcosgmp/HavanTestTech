using HavanTestTech.Application.Abstractions;

namespace HavanTestTech.Application.Todos;

public sealed class GetAllTodosHandler(ITodoRepository repository)
{
    public async Task<IReadOnlyList<TodoResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var items = await repository.GetAllAsync(cancellationToken);
        return items.Select(TodoResponse.From).ToList();
    }
}