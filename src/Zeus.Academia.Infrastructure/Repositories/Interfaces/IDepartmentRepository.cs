using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for Department entities with specialized operations.
/// </summary>
public interface IDepartmentRepository : IRepository<Department>
{
    /// <summary>
    /// Gets departments by university asynchronously.
    /// </summary>
    /// <param name="universityId">The university identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of departments in the specified university</returns>
    Task<IEnumerable<Department>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets department by code asynchronously.
    /// </summary>
    /// <param name="deptCode">The department code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The department with the specified code or null</returns>
    Task<Department?> GetByCodeAsync(string deptCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches departments by name pattern asynchronously.
    /// </summary>
    /// <param name="namePattern">The name search pattern</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of departments matching the name pattern</returns>
    Task<IEnumerable<Department>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets departments with their academics count asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of departments with academic counts</returns>
    Task<IEnumerable<Department>> GetWithAcademicsCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets department with all related academics asynchronously.
    /// </summary>
    /// <param name="id">The department identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The department with academics or null</returns>
    Task<Department?> GetWithAcademicsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets department with subjects asynchronously.
    /// </summary>
    /// <param name="id">The department identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The department with subjects or null</returns>
    Task<Department?> GetWithSubjectsAsync(int id, CancellationToken cancellationToken = default);
}