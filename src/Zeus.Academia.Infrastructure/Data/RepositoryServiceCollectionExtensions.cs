using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zeus.Academia.Infrastructure.Data.Repositories;

namespace Zeus.Academia.Infrastructure.Data;

/// <summary>
/// Extension methods for configuring repository services in dependency injection.
/// Task 6: Repository Pattern Implementation - DI registration for repositories.
/// </summary>
public static class RepositoryServiceCollectionExtensions
{
    /// <summary>
    /// Adds repository pattern services to the service collection.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register Unit of Work
        services.TryAddScoped<IUnitOfWork, UnitOfWork>();

        // Register generic repository
        services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register specific repositories
        services.TryAddScoped<IAcademicRepository, AcademicRepository>();
        services.TryAddScoped<IDepartmentRepository, DepartmentRepository>();
        services.TryAddScoped<ISubjectRepository, SubjectRepository>();

        return services;
    }

    /// <summary>
    /// Adds all Infrastructure data services including repositories and Unit of Work.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddInfrastructureData(this IServiceCollection services)
    {
        // Add repositories
        services.AddRepositories();

        // Add database initializer if not already registered
        services.TryAddScoped<DatabaseInitializer>();

        return services;
    }
}