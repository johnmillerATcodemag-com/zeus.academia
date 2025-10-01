using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Data.Repositories;

/// <summary>
/// Repository interface for Department entities with domain-specific operations.
/// Task 6: Repository Pattern Implementation - Department-specific repository contract.
/// </summary>
public interface IDepartmentRepository : IRepository<Department>
{
    /// <summary>
    /// Gets a department by its code.
    /// </summary>
    /// <param name="code">Department code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Department if found, null otherwise</returns>
    Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets departments by university.
    /// </summary>
    /// <param name="universityId">University identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of departments in the university</returns>
    Task<IEnumerable<Department>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets departments with their chair information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of departments with chair data</returns>
    Task<IEnumerable<Department>> GetWithChairAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets departments with their academics count.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of departments with academic counts</returns>
    Task<IEnumerable<DepartmentWithCount>> GetWithAcademicCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets departments by name (partial match).
    /// </summary>
    /// <param name="namePattern">Name pattern to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of departments matching the name pattern</returns>
    Task<IEnumerable<Department>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a department code is available.
    /// </summary>
    /// <param name="code">Department code to check</param>
    /// <param name="excludeName">Department Name to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if available, false otherwise</returns>
    Task<bool> IsCodeAvailableAsync(string code, string? excludeName = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO for department with academic count.
/// </summary>
public class DepartmentWithCount
{
    public Department Department { get; set; } = null!;
    public int AcademicCount { get; set; }
}