using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface for enrollment history tracking services
/// </summary>
public interface IEnrollmentHistoryService
{
    /// <summary>
    /// Records an enrollment event in the history
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <param name="eventType">The type of enrollment event</param>
    /// <param name="newStatus">The new enrollment status</param>
    /// <param name="previousStatus">The previous enrollment status (if applicable)</param>
    /// <param name="reason">Reason for the event</param>
    /// <param name="notes">Additional notes</param>
    /// <param name="processedBy">Who processed the event</param>
    /// <param name="applicationId">Associated application ID (if applicable)</param>
    /// <param name="departmentName">Department involved</param>
    /// <param name="program">Program involved</param>
    /// <param name="academicTerm">Academic term</param>
    /// <param name="academicYear">Academic year</param>
    /// <returns>The created enrollment history record</returns>
    Task<EnrollmentHistory> RecordEnrollmentEventAsync(
        int studentEmpNr,
        EnrollmentEventType eventType,
        EnrollmentStatus newStatus,
        EnrollmentStatus? previousStatus = null,
        string? reason = null,
        string? notes = null,
        string? processedBy = null,
        int? applicationId = null,
        string? departmentName = null,
        string? program = null,
        string? academicTerm = null,
        int? academicYear = null);

    /// <summary>
    /// Gets enrollment history for a specific student
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of enrollment history records</returns>
    Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetStudentEnrollmentHistoryAsync(
        int studentEmpNr, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets enrollment history by event type
    /// </summary>
    /// <param name="eventType">The event type to filter by</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of enrollment history records</returns>
    Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryByEventTypeAsync(
        EnrollmentEventType eventType, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets enrollment history by date range
    /// </summary>
    /// <param name="startDate">Start date for filtering</param>
    /// <param name="endDate">End date for filtering</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of enrollment history records</returns>
    Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryByDateRangeAsync(
        DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets enrollment history for a specific department
    /// </summary>
    /// <param name="departmentName">The department name</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of enrollment history records</returns>
    Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryByDepartmentAsync(
        string departmentName, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets enrollment history for a specific academic term
    /// </summary>
    /// <param name="academicTerm">The academic term</param>
    /// <param name="academicYear">The academic year</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of enrollment history records</returns>
    Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryByTermAsync(
        string academicTerm, int academicYear, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets the latest enrollment event for a student
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <returns>The most recent enrollment history record</returns>
    Task<EnrollmentHistory?> GetLatestEnrollmentEventAsync(int studentEmpNr);

    /// <summary>
    /// Gets enrollment history for an application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <returns>List of enrollment history records for the application</returns>
    Task<IEnumerable<EnrollmentHistory>> GetApplicationHistoryAsync(int applicationId);

    /// <summary>
    /// Searches enrollment history with multiple criteria
    /// </summary>
    /// <param name="studentEmpNr">Filter by student (optional)</param>
    /// <param name="eventType">Filter by event type (optional)</param>
    /// <param name="status">Filter by enrollment status (optional)</param>
    /// <param name="departmentName">Filter by department (optional)</param>
    /// <param name="program">Filter by program (optional)</param>
    /// <param name="academicTerm">Filter by academic term (optional)</param>
    /// <param name="academicYear">Filter by academic year (optional)</param>
    /// <param name="startDate">Start date for filtering (optional)</param>
    /// <param name="endDate">End date for filtering (optional)</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of enrollment history records</returns>
    Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> SearchHistoryAsync(
        int? studentEmpNr = null,
        EnrollmentEventType? eventType = null,
        EnrollmentStatus? status = null,
        string? departmentName = null,
        string? program = null,
        string? academicTerm = null,
        int? academicYear = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Gets enrollment statistics for a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Dictionary with event types and their counts</returns>
    Task<Dictionary<EnrollmentEventType, int>> GetEnrollmentStatisticsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets enrollment history records that require notifications
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>History records requiring notifications</returns>
    Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryRequiringNotificationAsync(
        int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Marks a history record as having notification sent
    /// </summary>
    /// <param name="historyId">The history record ID</param>
    /// <returns>True if successful</returns>
    Task<bool> MarkNotificationSentAsync(int historyId);

    /// <summary>
    /// Gets the total count of enrollment history records
    /// </summary>
    /// <returns>Total number of records</returns>
    Task<int> GetTotalHistoryCountAsync();

    /// <summary>
    /// Gets enrollment events for multiple students
    /// </summary>
    /// <param name="studentEmpNrs">List of student employee numbers</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of enrollment history records</returns>
    Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryForStudentsAsync(
        IEnumerable<int> studentEmpNrs, int pageNumber = 1, int pageSize = 10);
}