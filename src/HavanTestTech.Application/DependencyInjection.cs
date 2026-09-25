using HavanTestTech.Application.Todos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HavanTestTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registered only if the host has not provided its own clock (tests replace it).
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<CreateTodoHandler>();
        services.AddScoped<ChangeTodoStatusHandler>();
        services.AddScoped<GetAllTodosHandler>();
        services.AddScoped<GetActiveTodosHandler>();
        services.AddScoped<GetCompletedTodosHandler>();

        return services;
    }
}