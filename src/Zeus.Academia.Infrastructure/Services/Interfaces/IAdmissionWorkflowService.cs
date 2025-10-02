using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface for managing admission workflow processes
/// </summary>
public interface IAdmissionWorkflowService
{
    /// <summary>
    /// Initiates the review process for a submitted application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="reviewerName">Name of the reviewer</param>
    /// <param name="reviewNotes">Initial review notes</param>
    /// <returns>True if review was initiated successfully</returns>
    Task<bool> InitiateReviewAsync(int applicationId, string reviewerName, string? reviewNotes = null);

    /// <summary>
    /// Marks an application as requiring additional documents
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="requiredDocuments">List of required document types</param>
    /// <param name="reviewerName">Name of the reviewer</param>
    /// <param name="notes">Notes about missing documents</param>
    /// <returns>True if update was successful</returns>
    Task<bool> RequestAdditionalDocumentsAsync(int applicationId, List<string> requiredDocuments,
        string reviewerName, string? notes = null);

    /// <summary>
    /// Puts an application on hold with specified reason
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="holdReason">Reason for placing on hold</param>
    /// <param name="reviewerName">Name of the reviewer</param>
    /// <param name="expectedResolutionDate">Expected date to resolve hold</param>
    /// <returns>True if hold was applied successfully</returns>
    Task<bool> PlaceOnHoldAsync(int applicationId, string holdReason, string reviewerName,
        DateTime? expectedResolutionDate = null);

    /// <summary>
    /// Removes hold status from an application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="resolutionNotes">Notes about hold resolution</param>
    /// <param name="reviewerName">Name of the reviewer</param>
    /// <returns>True if hold was removed successfully</returns>
    Task<bool> RemoveHoldAsync(int applicationId, string resolutionNotes, string reviewerName);

    /// <summary>
    /// Processes a complete admission decision workflow
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="decision">The admission decision</param>
    /// <param name="decisionReason">Detailed reason for the decision</param>
    /// <param name="decisionMadeBy">Person making the decision</param>
    /// <param name="conditionalRequirements">Any conditional requirements (for conditional admission)</param>
    /// <param name="notifyApplicant">Whether to trigger notification to applicant</param>
    /// <returns>True if decision was processed successfully</returns>
    Task<bool> ProcessCompleteAdmissionDecisionAsync(int applicationId, AdmissionDecision decision,
        string decisionReason, string decisionMadeBy, string? conditionalRequirements = null,
        bool notifyApplicant = true);

    /// <summary>
    /// Gets applications requiring immediate attention (overdue reviews, missing documents, etc.)
    /// </summary>
    /// <param name="daysOverdue">Number of days past due date to consider overdue</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Applications requiring attention</returns>
    Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsRequiringAttentionAsync(
        int daysOverdue = 7, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets admission statistics for a specific time period
    /// </summary>
    /// <param name="startDate">Start date for statistics</param>
    /// <param name="endDate">End date for statistics</param>
    /// <param name="departmentName">Optional department filter</param>
    /// <returns>Admission statistics</returns>
    Task<AdmissionStatistics> GetAdmissionStatsAsync(DateTime startDate, DateTime endDate,
        string? departmentName = null);

    /// <summary>
    /// Validates if an application is ready for admission decision
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <returns>Validation result with any issues found</returns>
    Task<AdmissionValidationResult> ValidateReadyForDecisionAsync(int applicationId);

    /// <summary>
    /// Expedites processing for high-priority applications
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="expediteReason">Reason for expediting</param>
    /// <param name="requestedBy">Person requesting expedited processing</param>
    /// <returns>True if expedited successfully</returns>
    Task<bool> ExpediteApplicationAsync(int applicationId, string expediteReason, string requestedBy);

    /// <summary>
    /// Gets a timeline of all events for an application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <returns>Chronological list of application events</returns>
    Task<IEnumerable<ApplicationEvent>> GetApplicationTimelineAsync(int applicationId);
}

/// <summary>
/// Statistics for admission decisions
/// </summary>
public class AdmissionStatistics
{
    public int TotalApplications { get; set; }
    public int AdmittedCount { get; set; }
    public int RejectedCount { get; set; }
    public int WaitlistedCount { get; set; }
    public int ConditionallyAdmittedCount { get; set; }
    public int PendingCount { get; set; }
    public decimal AdmissionRate => TotalApplications > 0 ? (decimal)AdmittedCount / TotalApplications * 100 : 0;
    public decimal AverageProcessingDays { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

/// <summary>
/// Result of admission readiness validation
/// </summary>
public class AdmissionValidationResult
{
    public bool IsReadyForDecision { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool AllDocumentsSubmitted { get; set; }
    public bool AllDocumentsVerified { get; set; }
    public DateTime? EarliestDecisionDate { get; set; }
}

/// <summary>
/// Application event for timeline tracking
/// </summary>
public class ApplicationEvent
{
    public DateTime EventDate { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ProcessedBy { get; set; }
    public ApplicationStatus? StatusChange { get; set; }
    public AdmissionDecision? DecisionChange { get; set; }
}