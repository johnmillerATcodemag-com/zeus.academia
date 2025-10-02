using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for managing admission workflow processes
/// </summary>
public class AdmissionWorkflowService : IAdmissionWorkflowService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<AdmissionWorkflowService> _logger;
    private readonly IEnrollmentApplicationService _applicationService;
    private readonly IEnrollmentHistoryService _historyService;

    public AdmissionWorkflowService(
        AcademiaDbContext context,
        ILogger<AdmissionWorkflowService> logger,
        IEnrollmentApplicationService applicationService,
        IEnrollmentHistoryService historyService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        _historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));
    }

    public async Task<bool> InitiateReviewAsync(int applicationId, string reviewerName, string? reviewNotes = null)
    {
        _logger.LogInformation("Initiating review for application {ApplicationId} by {ReviewerName}",
            applicationId, reviewerName);

        var application = await _applicationService.GetApplicationByIdAsync(applicationId);
        if (application == null)
        {
            _logger.LogWarning("Application {ApplicationId} not found", applicationId);
            return false;
        }

        if (application.Status != ApplicationStatus.Submitted)
        {
            _logger.LogWarning("Application {ApplicationId} is not in Submitted status, current status: {Status}",
                applicationId, application.Status);
            return false;
        }

        // Update application status
        var success = await _applicationService.UpdateApplicationStatusAsync(
            applicationId,
            ApplicationStatus.UnderReview,
            $"Review initiated by {reviewerName}. {reviewNotes ?? ""}".Trim());

        if (success && application.ApplicantEmpNr.HasValue)
        {
            // Record in enrollment history
            await _historyService.RecordEnrollmentEventAsync(
                application.ApplicantEmpNr.Value,
                EnrollmentEventType.ApplicationReviewed,
                EnrollmentStatus.Applied,
                null,
                "Application review initiated",
                reviewNotes,
                reviewerName,
                applicationId);
        }

        _logger.LogInformation("Review initiated successfully for application {ApplicationId}", applicationId);
        return success;
    }

    public async Task<bool> RequestAdditionalDocumentsAsync(int applicationId, List<string> requiredDocuments,
        string reviewerName, string? notes = null)
    {
        _logger.LogInformation("Requesting additional documents for application {ApplicationId}", applicationId);

        var application = await _applicationService.GetApplicationByIdAsync(applicationId);
        if (application == null) return false;

        var documentsText = string.Join(", ", requiredDocuments);
        var statusNotes = $"Additional documents required: {documentsText}. {notes ?? ""}".Trim();

        var success = await _applicationService.UpdateApplicationStatusAsync(
            applicationId,
            ApplicationStatus.IncompleteDocuments,
            statusNotes);

        if (success && application.ApplicantEmpNr.HasValue)
        {
            // Record in enrollment history
            await _historyService.RecordEnrollmentEventAsync(
                application.ApplicantEmpNr.Value,
                EnrollmentEventType.ApplicationReviewed,
                EnrollmentStatus.Applied,
                null,
                "Additional documents requested",
                statusNotes,
                reviewerName,
                applicationId);
        }

        return success;
    }

    public async Task<bool> PlaceOnHoldAsync(int applicationId, string holdReason, string reviewerName,
        DateTime? expectedResolutionDate = null)
    {
        _logger.LogInformation("Placing application {ApplicationId} on hold", applicationId);

        var application = await _applicationService.GetApplicationByIdAsync(applicationId);
        if (application == null) return false;

        var holdNotes = $"Hold reason: {holdReason}";
        if (expectedResolutionDate.HasValue)
        {
            holdNotes += $" | Expected resolution: {expectedResolutionDate.Value:yyyy-MM-dd}";
        }

        var success = await _applicationService.UpdateApplicationStatusAsync(
            applicationId,
            ApplicationStatus.OnHold,
            holdNotes);

        if (success && application.ApplicantEmpNr.HasValue)
        {
            // Record in enrollment history
            await _historyService.RecordEnrollmentEventAsync(
                application.ApplicantEmpNr.Value,
                EnrollmentEventType.ApplicationReviewed,
                EnrollmentStatus.Applied,
                null,
                "Application placed on hold",
                holdNotes,
                reviewerName,
                applicationId);
        }

        return success;
    }

    public async Task<bool> RemoveHoldAsync(int applicationId, string resolutionNotes, string reviewerName)
    {
        _logger.LogInformation("Removing hold from application {ApplicationId}", applicationId);

        var application = await _applicationService.GetApplicationByIdAsync(applicationId);
        if (application == null) return false;

        if (application.Status != ApplicationStatus.OnHold)
        {
            _logger.LogWarning("Application {ApplicationId} is not on hold, current status: {Status}",
                applicationId, application.Status);
            return false;
        }

        var statusNotes = $"Hold removed: {resolutionNotes}";

        var success = await _applicationService.UpdateApplicationStatusAsync(
            applicationId,
            ApplicationStatus.UnderReview,
            statusNotes);

        if (success && application.ApplicantEmpNr.HasValue)
        {
            // Record in enrollment history
            await _historyService.RecordEnrollmentEventAsync(
                application.ApplicantEmpNr.Value,
                EnrollmentEventType.ApplicationReviewed,
                EnrollmentStatus.Applied,
                null,
                "Hold removed from application",
                statusNotes,
                reviewerName,
                applicationId);
        }

        return success;
    }

    public async Task<bool> ProcessCompleteAdmissionDecisionAsync(int applicationId, AdmissionDecision decision,
        string decisionReason, string decisionMadeBy, string? conditionalRequirements = null,
        bool notifyApplicant = true)
    {
        _logger.LogInformation("Processing complete admission decision for application {ApplicationId}: {Decision}",
            applicationId, decision);

        // Validate application is ready for decision
        var validation = await ValidateReadyForDecisionAsync(applicationId);
        if (!validation.IsReadyForDecision)
        {
            _logger.LogWarning("Application {ApplicationId} is not ready for decision. Issues: {Issues}",
                applicationId, string.Join(", ", validation.Issues));
            return false;
        }

        var application = await _applicationService.GetApplicationByIdAsync(applicationId);
        if (application == null) return false;

        // Process the admission decision
        var success = await _applicationService.ProcessAdmissionDecisionAsync(
            applicationId, decision, decisionReason, decisionMadeBy);

        if (!success) return false;

        // Add conditional requirements if applicable
        if (decision == AdmissionDecision.ConditionallyAdmitted && !string.IsNullOrWhiteSpace(conditionalRequirements))
        {
            await _applicationService.UpdateApplicationStatusAsync(
                applicationId,
                ApplicationStatus.Approved,
                $"Conditional requirements: {conditionalRequirements}");
        }

        // Record detailed enrollment history
        if (application.ApplicantEmpNr.HasValue)
        {
            var historyReason = decision switch
            {
                AdmissionDecision.Admitted => "Application approved for admission",
                AdmissionDecision.ConditionallyAdmitted => "Application conditionally approved for admission",
                AdmissionDecision.Rejected => "Application rejected",
                AdmissionDecision.Waitlisted => "Application placed on waitlist",
                AdmissionDecision.Deferred => "Application deferred to next term",
                _ => "Admission decision processed"
            };

            var newStatus = decision switch
            {
                AdmissionDecision.Admitted => EnrollmentStatus.Admitted,
                AdmissionDecision.ConditionallyAdmitted => EnrollmentStatus.Admitted,
                AdmissionDecision.Rejected => EnrollmentStatus.Dismissed,
                _ => EnrollmentStatus.Applied
            };

            await _historyService.RecordEnrollmentEventAsync(
                application.ApplicantEmpNr.Value,
                EnrollmentEventType.AdmissionDecision,
                newStatus,
                EnrollmentStatus.Applied,
                historyReason,
                $"Decision: {decision} | Reason: {decisionReason}",
                decisionMadeBy,
                applicationId);
        }

        // TODO: Implement notification service call if notifyApplicant is true
        if (notifyApplicant)
        {
            _logger.LogInformation("Notification would be sent to applicant for application {ApplicationId}", applicationId);
        }

        _logger.LogInformation("Complete admission decision processed successfully for application {ApplicationId}",
            applicationId);
        return true;
    }

    public async Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsRequiringAttentionAsync(
        int daysOverdue = 7, int pageNumber = 1, int pageSize = 10)
    {
        _logger.LogDebug("Getting applications requiring attention with {DaysOverdue} days overdue threshold",
            daysOverdue);

        var overdueDate = DateTime.UtcNow.AddDays(-daysOverdue);

        var query = _context.EnrollmentApplications
            .Include(ea => ea.Applicant)
            .Include(ea => ea.Department)
            .Where(ea =>
                (ea.Status == ApplicationStatus.Submitted && ea.ApplicationDate <= overdueDate) ||
                (ea.Status == ApplicationStatus.UnderReview && ea.ApplicationDate <= overdueDate.AddDays(-3)) ||
                ea.Status == ApplicationStatus.IncompleteDocuments ||
                (ea.Status == ApplicationStatus.OnHold && ea.ApplicationDate <= overdueDate.AddDays(-14)));

        var totalCount = await query.CountAsync();

        var applications = await query
            .OrderBy(ea => ea.Status == ApplicationStatus.IncompleteDocuments ? 1 : 2)
            .ThenBy(ea => ea.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (applications, totalCount);
    }

    public async Task<AdmissionStatistics> GetAdmissionStatsAsync(DateTime startDate, DateTime endDate,
        string? departmentName = null)
    {
        _logger.LogDebug("Getting admission statistics from {StartDate} to {EndDate}", startDate, endDate);

        var query = _context.EnrollmentApplications.Where(ea =>
            ea.ApplicationDate >= startDate && ea.ApplicationDate <= endDate);

        if (!string.IsNullOrWhiteSpace(departmentName))
        {
            query = query.Where(ea => ea.DepartmentName == departmentName);
        }

        var applications = await query.ToListAsync();
        var decisionsWithDates = applications.Where(a => a.DecisionDate.HasValue).ToList();

        var stats = new AdmissionStatistics
        {
            TotalApplications = applications.Count,
            AdmittedCount = applications.Count(a => a.Decision == AdmissionDecision.Admitted),
            RejectedCount = applications.Count(a => a.Decision == AdmissionDecision.Rejected),
            WaitlistedCount = applications.Count(a => a.Decision == AdmissionDecision.Waitlisted),
            ConditionallyAdmittedCount = applications.Count(a => a.Decision == AdmissionDecision.ConditionallyAdmitted),
            PendingCount = applications.Count(a => !a.Decision.HasValue),
            DepartmentName = departmentName,
            StartDate = startDate,
            EndDate = endDate
        };

        if (decisionsWithDates.Any())
        {
            var avgDays = decisionsWithDates
                .Select(a => (a.DecisionDate!.Value - a.ApplicationDate).TotalDays)
                .Average();
            stats.AverageProcessingDays = (decimal)Math.Round(avgDays, 1);
        }

        return stats;
    }

    public async Task<AdmissionValidationResult> ValidateReadyForDecisionAsync(int applicationId)
    {
        _logger.LogDebug("Validating application {ApplicationId} readiness for decision", applicationId);

        var result = new AdmissionValidationResult();
        var application = await _applicationService.GetApplicationByIdAsync(applicationId);

        if (application == null)
        {
            result.Issues.Add("Application not found");
            return result;
        }

        // Check application status
        if (application.Status == ApplicationStatus.Submitted)
        {
            result.Issues.Add("Application has not been reviewed yet");
        }
        else if (application.Status == ApplicationStatus.IncompleteDocuments)
        {
            result.Issues.Add("Application has incomplete documents");
        }
        else if (application.Status == ApplicationStatus.OnHold)
        {
            result.Issues.Add("Application is currently on hold");
        }

        // Check if documents are complete
        var allDocumentsSubmitted = await _applicationService.AreAllRequiredDocumentsSubmittedAsync(applicationId);
        result.AllDocumentsSubmitted = allDocumentsSubmitted;
        if (!allDocumentsSubmitted)
        {
            result.Issues.Add("Not all required documents have been submitted and verified");
        }

        // Check for minimum review period (e.g., 24 hours)
        var minReviewDate = application.ApplicationDate.AddDays(1);
        if (DateTime.UtcNow < minReviewDate)
        {
            result.Warnings.Add($"Application submitted recently, consider waiting until {minReviewDate:yyyy-MM-dd} for decision");
            result.EarliestDecisionDate = minReviewDate;
        }

        // Check if already has a decision
        if (application.Decision.HasValue)
        {
            result.Issues.Add($"Application already has a decision: {application.Decision}");
        }

        result.IsReadyForDecision = !result.Issues.Any();
        return result;
    }

    public async Task<bool> ExpediteApplicationAsync(int applicationId, string expediteReason, string requestedBy)
    {
        _logger.LogInformation("Expediting application {ApplicationId} requested by {RequestedBy}",
            applicationId, requestedBy);

        var application = await _applicationService.GetApplicationByIdAsync(applicationId);
        if (application == null) return false;

        // Update to high priority
        application.Priority = ApplicationPriority.Urgent;
        var expediteNotes = $"EXPEDITED by {requestedBy}: {expediteReason}";

        var success = await _applicationService.UpdateApplicationStatusAsync(
            applicationId,
            application.Status,
            expediteNotes);

        if (success && application.ApplicantEmpNr.HasValue)
        {
            // Record in enrollment history
            await _historyService.RecordEnrollmentEventAsync(
                application.ApplicantEmpNr.Value,
                EnrollmentEventType.ApplicationReviewed,
                EnrollmentStatus.Applied,
                null,
                "Application expedited",
                expediteNotes,
                requestedBy,
                applicationId);
        }

        return success;
    }

    public async Task<IEnumerable<ApplicationEvent>> GetApplicationTimelineAsync(int applicationId)
    {
        _logger.LogDebug("Getting timeline for application {ApplicationId}", applicationId);

        var application = await _applicationService.GetApplicationByIdAsync(applicationId);
        if (application == null) return Enumerable.Empty<ApplicationEvent>();

        var events = new List<ApplicationEvent>();

        // Add application submission event
        events.Add(new ApplicationEvent
        {
            EventDate = application.ApplicationDate,
            EventType = "Application Submitted",
            Description = $"Application submitted for {application.Program} program",
            StatusChange = ApplicationStatus.Submitted
        });

        // Get enrollment history for this application
        if (application.ApplicantEmpNr.HasValue)
        {
            var (history, _) = await _historyService.GetStudentEnrollmentHistoryAsync(application.ApplicantEmpNr.Value);
            var applicationHistory = history.Where(h => h.ApplicationId == applicationId);

            foreach (var historyItem in applicationHistory.OrderBy(h => h.EventDate))
            {
                events.Add(new ApplicationEvent
                {
                    EventDate = historyItem.EventDate,
                    EventType = historyItem.EventType.ToString(),
                    Description = historyItem.Reason ?? "Status change",
                    ProcessedBy = historyItem.ProcessedBy
                });
            }
        }

        // Add decision event if available
        if (application.Decision.HasValue && application.DecisionDate.HasValue)
        {
            events.Add(new ApplicationEvent
            {
                EventDate = application.DecisionDate.Value,
                EventType = "Admission Decision",
                Description = $"Decision: {application.Decision} - {application.DecisionReason ?? "No reason provided"}",
                ProcessedBy = application.DecisionMadeBy,
                DecisionChange = application.Decision
            });
        }

        return events.OrderBy(e => e.EventDate);
    }
}