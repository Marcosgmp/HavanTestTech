using System.Collections.Concurrent;
using HavanTestTech.Application.Abstractions;
using HavanTestTech.Domain.Entities;

namespace HavanTestTech.Infrastructure.Persistence;

public sealed class InMemoryTodoRepository : ITodoRepository
{
    // The repository is a singleton shared by simultaneous requests,
    // so the storage must be safe for concurrent access.
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();

    public Task AddAsync(TodoItem item, CancellationToken cancellationToken)
    {
        _items[item.Id] = item;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TodoItem item, CancellationToken cancellationToken)
    {
        _items[item.Id] = item;
        return Task.CompletedTask;
    }

    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _items.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken) =>
        AsResult(_items.Values.OrderBy(item => item.CreatedAt));

    public Task<IReadOnlyList<TodoItem>> GetActiveAsync(CancellationToken cancellationToken) =>
        AsResult(_items.Values
            .Where(item => item.IsActive)
            .OrderBy(item => item.CreatedAt));

    public Task<IReadOnlyList<TodoItem>> GetCompletedBetweenAsync(
        DateTime startInclusive,
        DateTime endExclusive,
        CancellationToken cancellationToken) =>
        AsResult(_items.Values
            .Where(item => item.CompletedAt >= startInclusive && item.CompletedAt < endExclusive)
            .OrderBy(item => item.CompletedAt));

    private static Task<IReadOnlyList<TodoItem>> AsResult(IEnumerable<TodoItem> items) =>
        Task.FromResult<IReadOnlyList<TodoItem>>(items.ToList());
}