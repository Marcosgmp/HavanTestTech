using HavanTestTech.Domain.Entities;
using HavanTestTech.Domain.Enums;
using HavanTestTech.Domain.Exceptions;

namespace HavanTestTech.Tests.Domain;

public class TodoItemTests
{
    private static readonly DateTime CreatedAt = new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime ChangedAt = CreatedAt.AddHours(3);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("abcd")]
    [InlineData("  abcd  ")]
    public void Constructor_WithInvalidTitle_ThrowsDomainException(string? title)
    {
        Assert.Throws<DomainException>(() => new TodoItem(title, "description", CreatedAt));
    }

    [Fact]
    public void Constructor_WithValidData_StartsPendingWithoutCompletionDate()
    {
        var item = new TodoItem("Write report", null, CreatedAt);

        Assert.Equal(TodoStatus.Pending, item.Status);
        Assert.Null(item.CompletedAt);
        Assert.Equal(CreatedAt, item.CreatedAt);
        Assert.True(item.IsActive);
    }

    [Fact]
    public void ChangeStatus_ToCompleted_FillsCompletionDate()
    {
        var item = new TodoItem("Write report", null, CreatedAt);

        item.ChangeStatus(TodoStatus.Completed, ChangedAt);

        Assert.Equal(TodoStatus.Completed, item.Status);
        Assert.Equal(ChangedAt, item.CompletedAt);
        Assert.False(item.IsActive);
    }

    [Fact]
    public void ChangeStatus_ToInProgress_KeepsCompletionDateEmpty()
    {
        var item = new TodoItem("Write report", null, CreatedAt);

        item.ChangeStatus(TodoStatus.InProgress, ChangedAt);

        Assert.Equal(TodoStatus.InProgress, item.Status);
        Assert.Null(item.CompletedAt);
    }

    [Theory]
    [InlineData(TodoStatus.Pending)]
    [InlineData(TodoStatus.InProgress)]
    [InlineData(TodoStatus.Completed)]
    public void ChangeStatus_WhenAlreadyCompleted_ThrowsDomainException(TodoStatus newStatus)
    {
        var item = new TodoItem("Write report", null, CreatedAt);
        item.ChangeStatus(TodoStatus.Completed, ChangedAt);

        Assert.Throws<DomainException>(() => item.ChangeStatus(newStatus, ChangedAt.AddHours(1)));
    }
}
