using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for Academic entities with specialized operations.
/// </summary>
public interface IAcademicRepository : IRepository<Academic>
{
    /// <summary>
    /// Gets academics by department asynchronously.
    /// </summary>
    /// <param name="departmentName">The department name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of academics in the specified department</returns>
    Task<IEnumerable<Academic>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets academics by employee number asynchronously.
    /// </summary>
    /// <param name="empNr">The employee number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The academic with the specified employee number or null</returns>
    Task<Academic?> GetByEmployeeNumberAsync(string empNr, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches academics by name pattern asynchronously.
    /// </summary>
    /// <param name="namePattern">The name search pattern</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of academics matching the name pattern</returns>
    Task<IEnumerable<Academic>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets academics by rank asynchronously.
    /// </summary>
    /// <param name="rankId">The rank identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of academics with the specified rank</returns>
    Task<IEnumerable<Academic>> GetByRankAsync(int rankId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all professors asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of professor academics</returns>
    Task<IEnumerable<Professor>> GetProfessorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all teachers asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of teacher academics</returns>
    Task<IEnumerable<Teacher>> GetTeachersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all students asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of student academics</returns>
    Task<IEnumerable<Student>> GetStudentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all department chairs asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of academics who are department chairs</returns>
    Task<IEnumerable<Academic>> GetChairsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all teaching professors asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of teaching professor academics</returns>
    Task<IEnumerable<TeachingProf>> GetTeachingProfessorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an employee number is available asynchronously.
    /// </summary>
    /// <param name="employeeNumber">The employee number to check</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the employee number is available, false otherwise</returns>
    Task<bool> IsEmployeeNumberAvailableAsync(string employeeNumber, int? excludeId = null, CancellationToken cancellationToken = default);
}