using HavanTestTech.Application.Abstractions;
using HavanTestTech.Domain.Entities;

namespace HavanTestTech.Application.Todos;

public sealed record CreateTodoRequest(string? Title, string? Description);

public sealed class CreateTodoHandler(ITodoRepository repository, TimeProvider timeProvider)
{
    public async Task<TodoResponse> HandleAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        var item = new TodoItem(request.Title, request.Description, timeProvider.GetUtcNow().UtcDateTime);

        await repository.AddAsync(item, cancellationToken);

        return TodoResponse.From(item);
    }
}