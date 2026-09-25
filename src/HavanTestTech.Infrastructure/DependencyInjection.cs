using HavanTestTech.Application.Abstractions;
using HavanTestTech.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace HavanTestTech.Infrastructure;

public static class DependencyInjection
{
    // Concrete implementations stay internal to this project: the API only calls this method.
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
        return services;
    }
}