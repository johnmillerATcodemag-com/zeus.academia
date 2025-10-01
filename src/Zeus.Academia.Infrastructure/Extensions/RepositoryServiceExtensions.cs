using Microsoft.Extensions.DependencyInjection;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring repository services in the dependency injection container.
/// </summary>
public static class RepositoryServiceExtensions
{
    /// <summary>
    /// Adds repository services to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        // Register generic repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register specific repositories
        services.AddScoped<IAcademicRepository, AcademicRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();

        // Register Unit of Work
        services.AddScoped<Repositories.Interfaces.IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// Adds repository services with custom configuration to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configure">Configuration action for repository services</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services,
        Action<RepositoryServiceOptions> configure)
    {
        var options = new RepositoryServiceOptions();
        configure(options);

        if (options.EnableCaching)
        {
            // Add caching decorators if enabled
            services.Decorate<IAcademicRepository, CachedAcademicRepository>();
            services.Decorate<IDepartmentRepository, CachedDepartmentRepository>();
            services.Decorate<ISubjectRepository, CachedSubjectRepository>();
        }

        return services.AddRepositoryServices();
    }
}

/// <summary>
/// Configuration options for repository services.
/// </summary>
public class RepositoryServiceOptions
{
    /// <summary>
    /// Gets or sets whether to enable caching for repositories.
    /// </summary>
    public bool EnableCaching { get; set; } = false;

    /// <summary>
    /// Gets or sets the cache expiration time in minutes.
    /// </summary>
    public int CacheExpirationMinutes { get; set; } = 30;
}

// Placeholder implementations for cached repositories (would be implemented if caching is needed)
internal class CachedAcademicRepository : IAcademicRepository
{
    private readonly IAcademicRepository _repository;

    public CachedAcademicRepository(IAcademicRepository repository)
    {
        _repository = repository;
    }

    // Implement all interface methods by delegating to _repository
    // This is a simplified placeholder - full implementation would add caching logic
    public Task<IEnumerable<Academic>> GetAllAsync(CancellationToken cancellationToken = default) => _repository.GetAllAsync(cancellationToken);
    public Task<Academic?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => _repository.GetByIdAsync(id, cancellationToken);
    public Task<IEnumerable<Academic>> FindAsync(System.Linq.Expressions.Expression<Func<Academic, bool>> predicate, CancellationToken cancellationToken = default) => _repository.FindAsync(predicate, cancellationToken);
    public Task<Academic?> GetSingleAsync(System.Linq.Expressions.Expression<Func<Academic, bool>> predicate, CancellationToken cancellationToken = default) => _repository.GetSingleAsync(predicate, cancellationToken);
    public Task<Academic> AddAsync(Academic entity, CancellationToken cancellationToken = default) => _repository.AddAsync(entity, cancellationToken);
    public Task AddRangeAsync(IEnumerable<Academic> entities, CancellationToken cancellationToken = default) => _repository.AddRangeAsync(entities, cancellationToken);
    public Task<Academic> UpdateAsync(Academic entity, CancellationToken cancellationToken = default) => _repository.UpdateAsync(entity, cancellationToken);
    public Task RemoveAsync(Academic entity, CancellationToken cancellationToken = default) => _repository.RemoveAsync(entity, cancellationToken);
    public Task RemoveByIdAsync(int id, CancellationToken cancellationToken = default) => _repository.RemoveByIdAsync(id, cancellationToken);
    public Task RemoveRangeAsync(IEnumerable<Academic> entities, CancellationToken cancellationToken = default) => _repository.RemoveRangeAsync(entities, cancellationToken);
    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Academic, bool>>? predicate = null, CancellationToken cancellationToken = default) => _repository.CountAsync(predicate, cancellationToken);
    public Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<Academic, bool>> predicate, CancellationToken cancellationToken = default) => _repository.ExistsAsync(predicate, cancellationToken);
    public Task<IEnumerable<Academic>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) => _repository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
    public Task<IEnumerable<Academic>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default) => _repository.GetByDepartmentAsync(departmentName, cancellationToken);
    public Task<Academic?> GetByEmployeeNumberAsync(string empNr, CancellationToken cancellationToken = default) => _repository.GetByEmployeeNumberAsync(empNr, cancellationToken);
    public Task<IEnumerable<Academic>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default) => _repository.SearchByNameAsync(namePattern, cancellationToken);
    public Task<IEnumerable<Academic>> GetByRankAsync(int rankId, CancellationToken cancellationToken = default) => _repository.GetByRankAsync(rankId, cancellationToken);
    public Task<IEnumerable<Professor>> GetProfessorsAsync(CancellationToken cancellationToken = default) => _repository.GetProfessorsAsync(cancellationToken);
    public Task<IEnumerable<Teacher>> GetTeachersAsync(CancellationToken cancellationToken = default) => _repository.GetTeachersAsync(cancellationToken);
    public Task<IEnumerable<Student>> GetStudentsAsync(CancellationToken cancellationToken = default) => _repository.GetStudentsAsync(cancellationToken);
    public Task<IEnumerable<Academic>> GetChairsAsync(CancellationToken cancellationToken = default) => _repository.GetChairsAsync(cancellationToken);
    public Task<IEnumerable<TeachingProf>> GetTeachingProfessorsAsync(CancellationToken cancellationToken = default) => _repository.GetTeachingProfessorsAsync(cancellationToken);
    public Task<bool> IsEmployeeNumberAvailableAsync(string employeeNumber, int? excludeId = null, CancellationToken cancellationToken = default) => _repository.IsEmployeeNumberAvailableAsync(employeeNumber, excludeId, cancellationToken);
}

internal class CachedDepartmentRepository : IDepartmentRepository
{
    private readonly IDepartmentRepository _repository;

    public CachedDepartmentRepository(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    // Simplified placeholder implementation
    public Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default) => _repository.GetAllAsync(cancellationToken);
    public Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => _repository.GetByIdAsync(id, cancellationToken);
    public Task<IEnumerable<Department>> FindAsync(System.Linq.Expressions.Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken = default) => _repository.FindAsync(predicate, cancellationToken);
    public Task<Department?> GetSingleAsync(System.Linq.Expressions.Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken = default) => _repository.GetSingleAsync(predicate, cancellationToken);
    public Task<Department> AddAsync(Department entity, CancellationToken cancellationToken = default) => _repository.AddAsync(entity, cancellationToken);
    public Task AddRangeAsync(IEnumerable<Department> entities, CancellationToken cancellationToken = default) => _repository.AddRangeAsync(entities, cancellationToken);
    public Task<Department> UpdateAsync(Department entity, CancellationToken cancellationToken = default) => _repository.UpdateAsync(entity, cancellationToken);
    public Task RemoveAsync(Department entity, CancellationToken cancellationToken = default) => _repository.RemoveAsync(entity, cancellationToken);
    public Task RemoveByIdAsync(int id, CancellationToken cancellationToken = default) => _repository.RemoveByIdAsync(id, cancellationToken);
    public Task RemoveRangeAsync(IEnumerable<Department> entities, CancellationToken cancellationToken = default) => _repository.RemoveRangeAsync(entities, cancellationToken);
    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Department, bool>>? predicate = null, CancellationToken cancellationToken = default) => _repository.CountAsync(predicate, cancellationToken);
    public Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken = default) => _repository.ExistsAsync(predicate, cancellationToken);
    public Task<IEnumerable<Department>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) => _repository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
    public Task<IEnumerable<Department>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default) => _repository.GetByUniversityAsync(universityId, cancellationToken);
    public Task<Department?> GetByCodeAsync(string deptCode, CancellationToken cancellationToken = default) => _repository.GetByCodeAsync(deptCode, cancellationToken);
    public Task<IEnumerable<Department>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default) => _repository.SearchByNameAsync(namePattern, cancellationToken);
    public Task<IEnumerable<Department>> GetWithAcademicsCountAsync(CancellationToken cancellationToken = default) => _repository.GetWithAcademicsCountAsync(cancellationToken);
    public Task<Department?> GetWithAcademicsAsync(int id, CancellationToken cancellationToken = default) => _repository.GetWithAcademicsAsync(id, cancellationToken);
    public Task<Department?> GetWithSubjectsAsync(int id, CancellationToken cancellationToken = default) => _repository.GetWithSubjectsAsync(id, cancellationToken);
}

internal class CachedSubjectRepository : ISubjectRepository
{
    private readonly ISubjectRepository _repository;

    public CachedSubjectRepository(ISubjectRepository repository)
    {
        _repository = repository;
    }

    // Simplified placeholder implementation
    public Task<IEnumerable<Subject>> GetAllAsync(CancellationToken cancellationToken = default) => _repository.GetAllAsync(cancellationToken);
    public Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => _repository.GetByIdAsync(id, cancellationToken);
    public Task<IEnumerable<Subject>> FindAsync(System.Linq.Expressions.Expression<Func<Subject, bool>> predicate, CancellationToken cancellationToken = default) => _repository.FindAsync(predicate, cancellationToken);
    public Task<Subject?> GetSingleAsync(System.Linq.Expressions.Expression<Func<Subject, bool>> predicate, CancellationToken cancellationToken = default) => _repository.GetSingleAsync(predicate, cancellationToken);
    public Task<Subject> AddAsync(Subject entity, CancellationToken cancellationToken = default) => _repository.AddAsync(entity, cancellationToken);
    public Task AddRangeAsync(IEnumerable<Subject> entities, CancellationToken cancellationToken = default) => _repository.AddRangeAsync(entities, cancellationToken);
    public Task<Subject> UpdateAsync(Subject entity, CancellationToken cancellationToken = default) => _repository.UpdateAsync(entity, cancellationToken);
    public Task RemoveAsync(Subject entity, CancellationToken cancellationToken = default) => _repository.RemoveAsync(entity, cancellationToken);
    public Task RemoveByIdAsync(int id, CancellationToken cancellationToken = default) => _repository.RemoveByIdAsync(id, cancellationToken);
    public Task RemoveRangeAsync(IEnumerable<Subject> entities, CancellationToken cancellationToken = default) => _repository.RemoveRangeAsync(entities, cancellationToken);
    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Subject, bool>>? predicate = null, CancellationToken cancellationToken = default) => _repository.CountAsync(predicate, cancellationToken);
    public Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<Subject, bool>> predicate, CancellationToken cancellationToken = default) => _repository.ExistsAsync(predicate, cancellationToken);
    public Task<IEnumerable<Subject>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) => _repository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
    public Task<IEnumerable<Subject>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default) => _repository.GetByDepartmentAsync(departmentName, cancellationToken);
    public Task<Subject?> GetByCodeAsync(string subjectCode, CancellationToken cancellationToken = default) => _repository.GetByCodeAsync(subjectCode, cancellationToken);
    public Task<IEnumerable<Subject>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default) => _repository.SearchByNameAsync(namePattern, cancellationToken);
    public Task<IEnumerable<Subject>> GetByCreditRangeAsync(int minCredits, int maxCredits, CancellationToken cancellationToken = default) => _repository.GetByCreditRangeAsync(minCredits, maxCredits, cancellationToken);
    public Task<IEnumerable<Subject>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default) => _repository.GetByTeacherAsync(teacherId, cancellationToken);
    public Task<Subject?> GetWithTeachingAssignmentsAsync(int id, CancellationToken cancellationToken = default) => _repository.GetWithTeachingAssignmentsAsync(id, cancellationToken);
}

// Extension to support decorator pattern (would need Scrutor package in real implementation)
internal static class DecoratorServiceExtensions
{
    public static IServiceCollection Decorate<TInterface, TDecorator>(this IServiceCollection services)
        where TInterface : class
        where TDecorator : class, TInterface
    {
        // This is a simplified placeholder - in real implementation you'd use Scrutor package
        // services.Decorate<TInterface, TDecorator>();
        return services;
    }
}