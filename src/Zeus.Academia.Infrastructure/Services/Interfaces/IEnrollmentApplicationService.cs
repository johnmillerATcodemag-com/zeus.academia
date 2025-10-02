using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface for enrollment application management services
/// </summary>
public interface IEnrollmentApplicationService
{
    /// <summary>
    /// Submits a new enrollment application
    /// </summary>
    /// <param name="application">The enrollment application to submit</param>
    /// <returns>The submitted application with assigned ID</returns>
    Task<EnrollmentApplication> SubmitApplicationAsync(EnrollmentApplication application);

    /// <summary>
    /// Gets an enrollment application by ID
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <returns>The enrollment application if found</returns>
    Task<EnrollmentApplication?> GetApplicationByIdAsync(int applicationId);

    /// <summary>
    /// Gets all applications with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of applications</returns>
    Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsAsync(
        int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Searches applications by various criteria
    /// </summary>
    /// <param name="status">Filter by application status</param>
    /// <param name="departmentName">Filter by department</param>
    /// <param name="program">Filter by program</param>
    /// <param name="academicTerm">Filter by academic term</param>
    /// <param name="academicYear">Filter by academic year</param>
    /// <param name="searchTerm">General search term (name, email)</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of applications</returns>
    Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> SearchApplicationsAsync(
        ApplicationStatus? status = null,
        string? departmentName = null,
        string? program = null,
        string? academicTerm = null,
        int? academicYear = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Updates an enrollment application
    /// </summary>
    /// <param name="application">The application to update</param>
    /// <returns>The updated application</returns>
    Task<EnrollmentApplication> UpdateApplicationAsync(EnrollmentApplication application);

    /// <summary>
    /// Processes an admission decision for an application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="decision">The admission decision</param>
    /// <param name="reason">Reason for the decision</param>
    /// <param name="decisionMadeBy">Who made the decision</param>
    /// <returns>True if the decision was processed successfully</returns>
    Task<bool> ProcessAdmissionDecisionAsync(int applicationId, AdmissionDecision decision,
        string? reason = null, string? decisionMadeBy = null);

    /// <summary>
    /// Updates the status of an application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="newStatus">The new status</param>
    /// <param name="notes">Optional notes about the status change</param>
    /// <returns>True if the status was updated successfully</returns>
    Task<bool> UpdateApplicationStatusAsync(int applicationId, ApplicationStatus newStatus, string? notes = null);

    /// <summary>
    /// Adds a document to an application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="document">The document to add</param>
    /// <returns>The added document</returns>
    Task<ApplicationDocument> AddDocumentToApplicationAsync(int applicationId, ApplicationDocument document);

    /// <summary>
    /// Gets all documents for an application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <returns>List of documents</returns>
    Task<IEnumerable<ApplicationDocument>> GetApplicationDocumentsAsync(int applicationId);

    /// <summary>
    /// Verifies a document
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="verifiedBy">Who verified the document</param>
    /// <param name="notes">Verification notes</param>
    /// <returns>True if verification was successful</returns>
    Task<bool> VerifyDocumentAsync(int documentId, string verifiedBy, string? notes = null);

    /// <summary>
    /// Checks if all required documents have been submitted for an application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <returns>True if all required documents are submitted and verified</returns>
    Task<bool> AreAllRequiredDocumentsSubmittedAsync(int applicationId);

    /// <summary>
    /// Gets applications by status
    /// </summary>
    /// <param name="status">The application status</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of applications</returns>
    Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsByStatusAsync(
        ApplicationStatus status, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets applications requiring review (incomplete documents, etc.)
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Applications requiring attention</returns>
    Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsRequiringReviewAsync(
        int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets applications by department
    /// </summary>
    /// <param name="departmentName">The department name</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of applications</returns>
    Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsByDepartmentAsync(
        string departmentName, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets applications for a specific academic term
    /// </summary>
    /// <param name="academicTerm">The academic term</param>
    /// <param name="academicYear">The academic year</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of applications</returns>
    Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsByTermAsync(
        string academicTerm, int academicYear, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Withdraws an application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="reason">Reason for withdrawal</param>
    /// <returns>True if withdrawal was successful</returns>
    Task<bool> WithdrawApplicationAsync(int applicationId, string? reason = null);

    /// <summary>
    /// Checks if an application exists
    /// </summary>
    /// <param name="applicationId">The application ID to check</param>
    /// <returns>True if application exists</returns>
    Task<bool> ApplicationExistsAsync(int applicationId);

    /// <summary>
    /// Gets the total count of applications
    /// </summary>
    /// <returns>Total number of applications</returns>
    Task<int> GetTotalApplicationCountAsync();

    /// <summary>
    /// Gets applications by priority
    /// </summary>
    /// <param name="priority">The application priority</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of applications</returns>
    Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsByPriorityAsync(
        ApplicationPriority priority, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets applications for a specific student
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <param name="status">Optional status filter</param>
    /// <returns>List of applications for the student</returns>
    Task<IEnumerable<EnrollmentApplication>> GetApplicationsByStudentAsync(
        int studentEmpNr, ApplicationStatus? status = null);
}