using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service implementation for course access and enrollment validation operations.
/// Provides methods to check course access permissions and student enrollments.
/// </summary>
public class CourseService : ICourseService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<CourseService> _logger;

    /// <summary>
    /// Initializes a new instance of the CourseService class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    public CourseService(AcademiaDbContext context, ILogger<CourseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<bool> IsStudentEnrolledAsync(int studentId, string subjectCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if student {StudentId} is enrolled in course {SubjectCode}", studentId, subjectCode);

            var isEnrolled = await _context.Set<StudentEnrollment>()
                .AnyAsync(se => se.StudentEmpNr == studentId &&
                               se.SubjectCode == subjectCode &&
                               se.Status != null &&
                               se.Status.ToLower() == "enrolled",
                          cancellationToken);

            _logger.LogDebug("Student {StudentId} enrollment in course {SubjectCode}: {IsEnrolled}",
                studentId, subjectCode, isEnrolled);

            return isEnrolled;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking student enrollment for student {StudentId} in course {SubjectCode}",
                studentId, subjectCode);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DoesFacultyTeachCourseAsync(int facultyId, string subjectCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if faculty {FacultyId} teaches course {SubjectCode}", facultyId, subjectCode);

            var teaches = await _context.Set<Teaching>()
                .AnyAsync(t => t.AcademicEmpNr == facultyId &&
                              t.SubjectCode == subjectCode,
                          cancellationToken);

            _logger.LogDebug("Faculty {FacultyId} teaches course {SubjectCode}: {Teaches}",
                facultyId, subjectCode, teaches);

            return teaches;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking faculty teaching for faculty {FacultyId} in course {SubjectCode}",
                facultyId, subjectCode);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> GetStudentEnrolledCoursesAsync(int studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting enrolled courses for student {StudentId}", studentId);

            var courses = await _context.Set<StudentEnrollment>()
                .Where(se => se.StudentEmpNr == studentId &&
                            se.Status != null &&
                            se.Status.ToLower() == "enrolled")
                .Select(se => se.SubjectCode)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {CourseCount} enrolled courses for student {StudentId}",
                courses.Count, studentId);

            return courses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting enrolled courses for student {StudentId}", studentId);
            return new List<string>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> GetFacultyTaughtCoursesAsync(int facultyId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting taught courses for faculty {FacultyId}", facultyId);

            var courses = await _context.Set<Teaching>()
                .Where(t => t.AcademicEmpNr == facultyId)
                .Select(t => t.SubjectCode)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {CourseCount} taught courses for faculty {FacultyId}",
                courses.Count, facultyId);

            return courses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting taught courses for faculty {FacultyId}", facultyId);
            return new List<string>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DoesCourseExistAsync(string subjectCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if course {SubjectCode} exists", subjectCode);

            var exists = await _context.Set<Subject>()
                .AnyAsync(s => s.Code == subjectCode && s.IsActive, cancellationToken);

            _logger.LogDebug("Course {SubjectCode} exists: {Exists}", subjectCode, exists);

            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if course {SubjectCode} exists", subjectCode);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<Subject?> GetCourseAsync(string subjectCode, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting course information for {SubjectCode}", subjectCode);

            var subject = await _context.Set<Subject>()
                .FirstOrDefaultAsync(s => s.Code == subjectCode && s.IsActive, cancellationToken);

            if (subject != null)
            {
                _logger.LogDebug("Found course {SubjectCode}: {Title}", subjectCode, subject.Title);
            }
            else
            {
                _logger.LogDebug("Course {SubjectCode} not found", subjectCode);
            }

            return subject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course information for {SubjectCode}", subjectCode);
            return null;
        }
    }
}