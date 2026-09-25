using HavanTestTech.Domain.Entities;
using HavanTestTech.Domain.Enums;

namespace HavanTestTech.Application.Todos;

// A dedicated response type keeps the domain entity from leaking to API clients.
public sealed record TodoResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    TodoStatus Status)
{
    public static TodoResponse From(TodoItem item) =>
        new(item.Id, item.Title, item.Description, item.CreatedAt, item.CompletedAt, item.Status);
}