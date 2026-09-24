using HavanTestTech.Domain.Entities;

namespace HavanTestTech.Application.Abstractions;

/// <summary>
/// Persistence contract owned by the application layer and implemented by infrastructure.
/// List queries return items ordered by creation date, except for completed items,
/// which are ordered by completion date.
/// </summary>
public interface ITodoRepository
{
    Task AddAsync(TodoItem item, CancellationToken cancellationToken);

    Task UpdateAsync(TodoItem item, CancellationToken cancellationToken);

    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<TodoItem>> GetActiveAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<TodoItem>> GetCompletedBetweenAsync(
        DateTime startInclusive,
        DateTime endExclusive,
        CancellationToken cancellationToken);
}