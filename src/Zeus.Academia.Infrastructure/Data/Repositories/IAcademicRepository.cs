using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Data.Repositories;

/// <summary>
/// Repository interface for Academic entities with domain-specific operations.
/// Task 6: Repository Pattern Implementation - Academic-specific repository contract.
/// </summary>
public interface IAcademicRepository : IRepository<Academic>
{
    /// <summary>
    /// Gets an academic by employee number.
    /// </summary>
    /// <param name="employeeNumber">Employee number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Academic if found, null otherwise</returns>
    Task<Academic?> GetByEmployeeNumberAsync(string employeeNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets academics by department.
    /// </summary>
    /// <param name="departmentId">Department identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of academics in the department</returns>
    Task<IEnumerable<Academic>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets academics by rank.
    /// </summary>
    /// <param name="rankId">Rank identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of academics with the specified rank</returns>
    Task<IEnumerable<Academic>> GetByRankAsync(int rankId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets academics by name (partial match).
    /// </summary>
    /// <param name="namePattern">Name pattern to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of academics matching the name pattern</returns>
    Task<IEnumerable<Academic>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets academics with their department and rank information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of academics with related data</returns>
    Task<IEnumerable<Academic>> GetWithDepartmentAndRankAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets professors (academics with Professor discriminator).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of professors</returns>
    Task<IEnumerable<Academic>> GetProfessorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teaching professors.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of teaching professors</returns>
    Task<IEnumerable<Academic>> GetTeachingProfessorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets department chairs.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of department chairs</returns>
    Task<IEnumerable<Academic>> GetChairsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an employee number is available.
    /// </summary>
    /// <param name="employeeNumber">Employee number to check</param>
    /// <param name="excludeEmpNr">Academic Employee Number to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if available, false otherwise</returns>
    Task<bool> IsEmployeeNumberAvailableAsync(string employeeNumber, int? excludeEmpNr = null, CancellationToken cancellationToken = default);
}