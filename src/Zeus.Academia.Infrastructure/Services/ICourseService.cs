using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service interface for course access and enrollment validation operations.
/// Provides methods to check course access permissions and student enrollments.
/// </summary>
public interface ICourseService
{
    /// <summary>
    /// Checks if a student is enrolled in a specific course/subject.
    /// </summary>
    /// <param name="studentId">The student's academic ID</param>
    /// <param name="subjectCode">The subject/course code</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>True if the student is enrolled, false otherwise</returns>
    Task<bool> IsStudentEnrolledAsync(int studentId, string subjectCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a faculty member teaches a specific course/subject.
    /// </summary>
    /// <param name="facultyId">The faculty member's academic ID</param>
    /// <param name="subjectCode">The subject/course code</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>True if the faculty member teaches the course, false otherwise</returns>
    Task<bool> DoesFacultyTeachCourseAsync(int facultyId, string subjectCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all courses that a student is enrolled in.
    /// </summary>
    /// <param name="studentId">The student's academic ID</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>List of enrolled course codes</returns>
    Task<IEnumerable<string>> GetStudentEnrolledCoursesAsync(int studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all courses that a faculty member teaches.
    /// </summary>
    /// <param name="facultyId">The faculty member's academic ID</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>List of taught course codes</returns>
    Task<IEnumerable<string>> GetFacultyTaughtCoursesAsync(int facultyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a course/subject exists and is active.
    /// </summary>
    /// <param name="subjectCode">The subject/course code</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>True if the course exists and is active, false otherwise</returns>
    Task<bool> DoesCourseExistAsync(string subjectCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets course information by subject code.
    /// </summary>
    /// <param name="subjectCode">The subject/course code</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The subject entity if found, otherwise null</returns>
    Task<Subject?> GetCourseAsync(string subjectCode, CancellationToken cancellationToken = default);
}