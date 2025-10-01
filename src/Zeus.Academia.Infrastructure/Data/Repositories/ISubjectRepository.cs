using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Data.Repositories;

/// <summary>
/// Repository interface for Subject entities with domain-specific operations.
/// Task 6: Repository Pattern Implementation - Subject-specific repository contract.
/// </summary>
public interface ISubjectRepository : IRepository<Subject>
{
    /// <summary>
    /// Gets a subject by its code.
    /// </summary>
    /// <param name="code">Subject code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Subject if found, null otherwise</returns>
    Task<Subject?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects by department.
    /// </summary>
    /// <param name="departmentId">Department identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of subjects in the department</returns>
    Task<IEnumerable<Subject>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects by name (partial match).
    /// </summary>
    /// <param name="namePattern">Name pattern to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of subjects matching the name pattern</returns>
    Task<IEnumerable<Subject>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects with their department information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of subjects with department data</returns>
    Task<IEnumerable<Subject>> GetWithDepartmentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active subjects (assuming there's an IsActive property).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active subjects</returns>
    Task<IEnumerable<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a subject code is available.
    /// </summary>
    /// <param name="code">Subject code to check</param>
    /// <param name="excludeCode">Subject Code to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if available, false otherwise</returns>
    Task<bool> IsCodeAvailableAsync(string code, string? excludeCode = null, CancellationToken cancellationToken = default);
}