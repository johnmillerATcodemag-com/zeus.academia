using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Models;

namespace Zeus.Academia.Infrastructure.Services;

/// <    /// <summary>
/// Check degree progress for a student.
/// </summary>
/// <param name="studentId">Student ID</param>
/// <param name="degreeCode">Degree code</param>
/// <returns>Degree progress information</returns>

/// Interface for comprehensive course business operations including CRUD, 
/// equivalency management, transfer credit evaluation, capacity management, and analytics.
/// Extended from original course service to support Task 7 requirements.
/// </summary>
public interface ICourseService
{
    #region Legacy Course Access Methods (Preserved for backward compatibility)

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

    #endregion

    #region Core Course Operations

    /// <summary>
    /// Create a new course with comprehensive validation and business rules.
    /// </summary>
    /// <param name="course">Course to create</param>
    /// <returns>Created course with assigned ID</returns>
    Task<Course> CreateCourseAsync(Course course);

    /// <summary>
    /// Update an existing course with validation.
    /// </summary>
    /// <param name="course">Course to update</param>
    /// <returns>Updated course</returns>
    Task<Course> UpdateCourseAsync(Course course);

    /// <summary>
    /// Get course by ID with all related data.
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>Course if found</returns>
    Task<Course?> GetCourseByIdAsync(int courseId);

    /// <summary>
    /// Get all active courses with optional filtering.
    /// </summary>
    /// <param name="subjectCode">Optional subject filter</param>
    /// <param name="level">Optional level filter</param>
    /// <returns>List of active courses</returns>
    Task<List<Course>> GetActiveCoursesAsync(string? subjectCode = null, CourseLevel? level = null);

    /// <summary>
    /// Soft delete a course (mark as inactive).
    /// </summary>
    /// <param name="courseId">Course ID to delete</param>
    /// <returns>True if successful</returns>
    Task<bool> DeleteCourseAsync(int courseId);

    #endregion

    #region Prerequisite Validation

    /// <summary>
    /// Validate prerequisites for a student enrolling in a course.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="courseId">Course ID</param>
    /// <returns>Prerequisite validation result</returns>
    Task<PrerequisiteValidationResult> ValidatePrerequisitesAsync(int studentId, int courseId);

    /// <summary>
    /// Check if student can override prerequisite requirements.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="courseId">Course ID</param>
    /// <param name="overrideReason">Reason for override</param>
    /// <returns>True if override is allowed</returns>
    Task<bool> CanOverridePrerequisitesAsync(int studentId, int courseId, string overrideReason);

    #endregion

    #region Course Recommendations

    /// <summary>
    /// Recommend courses for a student based on their profile and progress.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="maxRecommendations">Maximum number of recommendations</param>
    /// <returns>List of course recommendations</returns>
    Task<List<CourseRecommendation>> RecommendCoursesAsync(int studentId, int maxRecommendations = 10);

    /// <summary>
    /// Get courses similar to a given course.
    /// </summary>
    /// <param name="courseId">Base course ID</param>
    /// <param name="maxSimilar">Maximum similar courses to return</param>
    /// <returns>List of similar courses</returns>
    Task<List<Course>> GetSimilarCoursesAsync(int courseId, int maxSimilar = 5);

    #endregion

    #region Degree Progress Tracking

    /// <summary>
    /// Check degree progress for a student.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="degreeCode">Degree program code</param>
    /// <returns>Degree progress information</returns>
    Task<Models.DegreeProgress> CheckDegreeProgressAsync(int studentId, string degreeCode);

    /// <summary>
    /// Get remaining courses needed for degree completion.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="degreeCode">Degree program code</param>
    /// <returns>List of remaining required courses</returns>
    Task<List<Course>> GetRemainingRequiredCoursesAsync(int studentId, string degreeCode);

    #endregion

    #region Course Equivalency Management

    /// <summary>
    /// Find course equivalency for an external course.
    /// </summary>
    /// <param name="externalCourse">External course information</param>
    /// <returns>Course equivalency if found</returns>
    Task<CourseEquivalency?> FindCourseEquivalencyAsync(ExternalCourse externalCourse);

    /// <summary>
    /// Create a new course equivalency mapping.
    /// </summary>
    /// <param name="equivalency">Equivalency to create</param>
    /// <returns>Created equivalency</returns>
    Task<CourseEquivalency> CreateCourseEquivalencyAsync(CourseEquivalency equivalency);

    /// <summary>
    /// Update an existing course equivalency.
    /// </summary>
    /// <param name="equivalency">Equivalency to update</param>
    /// <returns>Updated equivalency</returns>
    Task<CourseEquivalency> UpdateCourseEquivalencyAsync(CourseEquivalency equivalency);

    /// <summary>
    /// Get all equivalencies for an internal course.
    /// </summary>
    /// <param name="internalCourseId">Internal course ID</param>
    /// <returns>List of equivalencies</returns>
    Task<List<CourseEquivalency>> GetCourseEquivalenciesAsync(int internalCourseId);

    #endregion

    #region Capacity and Availability Management

    /// <summary>
    /// Get current availability information for a course.
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>Course availability information</returns>
    Task<CourseAvailability> GetCourseAvailabilityAsync(int courseId);

    /// <summary>
    /// Process an enrollment request with capacity management.
    /// </summary>
    /// <param name="enrollmentRequest">Enrollment request</param>
    /// <returns>Enrollment result</returns>
    Task<EnrollmentResult> ProcessEnrollmentRequestAsync(EnrollmentRequest enrollmentRequest);

    /// <summary>
    /// Get waitlist information for a course.
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of waitlist entries</returns>
    Task<List<CourseWaitlist>> GetCourseWaitlistAsync(int courseId);

    /// <summary>
    /// Process waitlist when a seat becomes available.
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>Student ID that was enrolled from waitlist, or null</returns>
    Task<int?> ProcessWaitlistAsync(int courseId);

    #endregion

    #region Course Search and Filtering

    /// <summary>
    /// Search courses with advanced criteria.
    /// </summary>
    /// <param name="searchCriteria">Search criteria</param>
    /// <returns>List of matching courses</returns>
    Task<List<Course>> SearchCoursesAsync(CourseSearchCriteria searchCriteria);

    /// <summary>
    /// Get courses by subject with hierarchy support.
    /// </summary>
    /// <param name="subjectCode">Subject code</param>
    /// <param name="includeSubjects">Whether to include child subjects</param>
    /// <returns>List of courses</returns>
    Task<List<Course>> GetCoursesBySubjectAsync(string subjectCode, bool includeSubjects = false);

    /// <summary>
    /// Get courses available in a specific semester.
    /// </summary>
    /// <param name="semester">Semester code</param>
    /// <param name="year">Academic year</param>
    /// <returns>List of available courses</returns>
    Task<List<Course>> GetCoursesForSemesterAsync(string semester, int year);

    #endregion

    #region Business Logic Validation

    /// <summary>
    /// Validate course business rules before creation/update.
    /// </summary>
    /// <param name="course">Course to validate</param>
    /// <returns>Validation result</returns>
    Task<CourseValidationResult> ValidateCourseBusinessRulesAsync(Course course);

    /// <summary>
    /// Check if course can be deleted (no active enrollments, etc.).
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>True if course can be deleted</returns>
    Task<bool> CanDeleteCourseAsync(int courseId);

    /// <summary>
    /// Validate course prerequisite chain for circular dependencies.
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>True if prerequisite chain is valid</returns>
    Task<bool> ValidatePrerequisiteChainAsync(int courseId);

    #endregion
}