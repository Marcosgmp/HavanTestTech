using HavanTestTech.Application.Exceptions;
using HavanTestTech.Application.Todos;
using HavanTestTech.Domain.Enums;
using HavanTestTech.Infrastructure.Persistence;

namespace HavanTestTech.Tests.Application;

public class TodoHandlersTests
{
    private readonly TestClock _clock = new(new DateTimeOffset(2026, 10, 1, 12, 0, 0, TimeSpan.Zero));
    private readonly CreateTodoHandler _create;
    private readonly ChangeTodoStatusHandler _changeStatus;
    private readonly GetActiveTodosHandler _getActive;
    private readonly GetCompletedTodosHandler _getCompleted;

    public TodoHandlersTests()
    {
        var repository = new InMemoryTodoRepository();
        _create = new CreateTodoHandler(repository, _clock);
        _changeStatus = new ChangeTodoStatusHandler(repository, _clock);
        _getActive = new GetActiveTodosHandler(repository);
        _getCompleted = new GetCompletedTodosHandler(repository);
    }

    [Fact]
    public async Task ChangeStatus_ToCompleted_UsesCurrentTimeAsCompletionDate()
    {
        var created = await CreateAsync("Write report");
        _clock.Advance(TimeSpan.FromHours(2));

        var updated = await _changeStatus.HandleAsync(created.Id, TodoStatus.Completed, default);

        Assert.Equal(_clock.GetUtcNow().UtcDateTime, updated.CompletedAt);
    }

    [Fact]
    public async Task ChangeStatus_WithUnknownId_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<TodoNotFoundException>(() =>
            _changeStatus.HandleAsync(Guid.NewGuid(), TodoStatus.InProgress, default));
    }

    [Fact]
    public async Task ChangeStatus_WithUndefinedStatus_ThrowsInvalidRequest()
    {
        var created = await CreateAsync("Write report");

        await Assert.ThrowsAsync<InvalidRequestException>(() =>
            _changeStatus.HandleAsync(created.Id, (TodoStatus)99, default));
    }
    [Fact]
    public async Task GetActive_ExcludesCompletedTasks()
    {
        // The clock advances between creations so each task gets a distinct CreatedAt,
        // making the expected ordering unambiguous.
        var pending = await CreateAsync("Pending task");
        _clock.Advance(TimeSpan.FromSeconds(1));
        var inProgress = await CreateAsync("Task in progress");
        _clock.Advance(TimeSpan.FromSeconds(1));
        var completed = await CreateAsync("Completed task");

        await _changeStatus.HandleAsync(inProgress.Id, TodoStatus.InProgress, default);
        await _changeStatus.HandleAsync(completed.Id, TodoStatus.Completed, default);

        var active = await _getActive.HandleAsync(default);

        Assert.Equal([pending.Id, inProgress.Id], active.Select(item => item.Id));
    }
    [Fact]
    public async Task GetCompleted_IncludesBothBoundaryDays()
    {
        // Completed on Oct 1, Oct 2 and Oct 3 respectively.
        var first = await CreateAndCompleteAsync("First task");
        _clock.Advance(TimeSpan.FromDays(1));
        var second = await CreateAndCompleteAsync("Second task");
        _clock.Advance(TimeSpan.FromDays(1));
        await CreateAndCompleteAsync("Third task");

        var range = await _getCompleted.HandleAsync(new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 2), default);
        var singleDay = await _getCompleted.HandleAsync(new DateOnly(2026, 10, 2), new DateOnly(2026, 10, 2), default);

        Assert.Equal([first.Id, second.Id], range.Select(item => item.Id));
        Assert.Equal([second.Id], singleDay.Select(item => item.Id));
    }

    [Fact]
    public async Task GetCompleted_WithStartAfterEnd_ThrowsInvalidRequest()
    {
        await Assert.ThrowsAsync<InvalidRequestException>(() =>
            _getCompleted.HandleAsync(new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 1), default));
    }

    private Task<TodoResponse> CreateAsync(string title) =>
        _create.HandleAsync(new CreateTodoRequest(title, "Description"), default);

    private async Task<TodoResponse> CreateAndCompleteAsync(string title)
    {
        var created = await CreateAsync(title);
        return await _changeStatus.HandleAsync(created.Id, TodoStatus.Completed, default);
    }
}
