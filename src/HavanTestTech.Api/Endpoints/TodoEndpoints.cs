using HavanTestTech.Application.Todos;
using HavanTestTech.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HavanTestTech.Api.Endpoints;

public sealed record ChangeStatusBody(TodoStatus Status);

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todos");

        group.MapPost("/", async (
            CreateTodoRequest request,
            CreateTodoHandler handler,
            CancellationToken cancellationToken) =>
        {
            var created = await handler.HandleAsync(request, cancellationToken);
            return Results.Created($"/api/todos/{created.Id}", created);
        })
        .WithName("CreateTodo")
        .WithSummary("Cria uma nova tarefa");

        group.MapGet("/", async (GetAllTodosHandler handler, CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(cancellationToken)))
        .WithName("GetAllTodos")
        .WithSummary("Lista todas as tarefas");

        group.MapGet("/active", async (GetActiveTodosHandler handler, CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(cancellationToken)))
        .WithName("GetActiveTodos")
        .WithSummary("Lista tarefas pendentes e em andamento");

        group.MapGet("/completed", async (
            [FromQuery(Name = "from")] DateOnly startDate,
            [FromQuery(Name = "to")] DateOnly endDate,
            GetCompletedTodosHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(startDate, endDate, cancellationToken)))
        .WithName("GetCompletedTodos")
        .WithSummary("Lista tarefas concluídas em um intervalo de datas");

        group.MapPatch("/{id:guid}/status", async (
            Guid id,
            ChangeStatusBody body,
            ChangeTodoStatusHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(id, body.Status, cancellationToken)))
        .WithName("ChangeTodoStatus")
        .WithSummary("Altera o status de uma tarefa");

        return app;
    }
}