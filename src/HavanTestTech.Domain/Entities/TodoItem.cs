using HavanTestTech.Domain.Enums;
using HavanTestTech.Domain.Exceptions;

namespace HavanTestTech.Domain.Entities;

public sealed class TodoItem
{
    public const int MinimumTitleLength = 5;

    public Guid Id { get; }
    public string Title { get; }
    public string Description { get; }
    public DateTime CreatedAt { get; }
    public DateTime? CompletedAt { get; private set; }
    public TodoStatus Status { get; private set; }

    public bool IsActive => Status != TodoStatus.Completed;

    // Timestamps are received as parameters instead of read from the system clock,
    // which keeps the entity deterministic and easy to test.
    public TodoItem(string? title, string? description, DateTime createdAt)
    {
        var normalizedTitle = title?.Trim();

        // Validating on construction guarantees that an invalid task never exists.
        if (normalizedTitle is null || normalizedTitle.Length < MinimumTitleLength)
        {
            throw new DomainException($"The title must have at least {MinimumTitleLength} characters.");
        }

        Id = Guid.NewGuid();
        Title = normalizedTitle;
        Description = description?.Trim() ?? string.Empty;
        CreatedAt = createdAt;
        Status = TodoStatus.Pending;
    }

    public void ChangeStatus(TodoStatus newStatus, DateTime changedAt)
    {
        if (Status == TodoStatus.Completed)
        {
            throw new DomainException("A completed task cannot have its status changed.");
        }

        Status = newStatus;

        if (newStatus == TodoStatus.Completed)
        {
            CompletedAt = changedAt;
        }
    }
}