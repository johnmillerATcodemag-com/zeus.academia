using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for Subject entities with specialized operations.
/// </summary>
public interface ISubjectRepository : IRepository<Subject>
{
    /// <summary>
    /// Gets subjects by department asynchronously.
    /// </summary>
    /// <param name="departmentName">The department name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of subjects in the specified department</returns>
    Task<IEnumerable<Subject>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subject by code asynchronously.
    /// </summary>
    /// <param name="subjectCode">The subject code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The subject with the specified code or null</returns>
    Task<Subject?> GetByCodeAsync(string subjectCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches subjects by name pattern asynchronously.
    /// </summary>
    /// <param name="namePattern">The name search pattern</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of subjects matching the name pattern</returns>
    Task<IEnumerable<Subject>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects by credit hours range asynchronously.
    /// </summary>
    /// <param name="minCredits">Minimum credit hours</param>
    /// <param name="maxCredits">Maximum credit hours</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of subjects within the credit range</returns>
    Task<IEnumerable<Subject>> GetByCreditRangeAsync(int minCredits, int maxCredits, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects taught by a specific teacher asynchronously.
    /// </summary>
    /// <param name="teacherId">The teacher identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of subjects taught by the teacher</returns>
    Task<IEnumerable<Subject>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subject with teaching assignments asynchronously.
    /// </summary>
    /// <param name="id">The subject identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The subject with teaching assignments or null</returns>
    Task<Subject?> GetWithTeachingAssignmentsAsync(int id, CancellationToken cancellationToken = default);
}