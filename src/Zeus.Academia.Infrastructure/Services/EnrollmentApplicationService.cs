using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for managing enrollment applications
/// </summary>
public class EnrollmentApplicationService : IEnrollmentApplicationService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<EnrollmentApplicationService> _logger;

    public EnrollmentApplicationService(AcademiaDbContext context, ILogger<EnrollmentApplicationService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<EnrollmentApplication> SubmitApplicationAsync(EnrollmentApplication application)
    {
        if (application == null)
            throw new ArgumentNullException(nameof(application));

        _logger.LogInformation("Submitting new enrollment application for {ApplicantName}", application.ApplicantName);

        // Validate required fields
        ValidateApplication(application);

        // Set defaults
        application.ApplicationDate = DateTime.UtcNow;
        application.Status = ApplicationStatus.Submitted;
        application.CreatedBy = "System";
        application.ModifiedBy = "System";

        _context.EnrollmentApplications.Add(application);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully submitted application with ID: {ApplicationId}", application.Id);
        return application;
    }

    public async Task<EnrollmentApplication?> GetApplicationByIdAsync(int applicationId)
    {
        _logger.LogDebug("Getting enrollment application by ID: {ApplicationId}", applicationId);

        return await _context.EnrollmentApplications
            .Include(ea => ea.Applicant)
            .Include(ea => ea.Department)
            .Include(ea => ea.Documents)
            .Include(ea => ea.EnrollmentHistory)
            .FirstOrDefaultAsync(ea => ea.Id == applicationId);
    }

    public async Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsAsync(
        int pageNumber = 1, int pageSize = 10)
    {
        _logger.LogDebug("Getting applications - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _context.EnrollmentApplications
            .Include(ea => ea.Applicant)
            .Include(ea => ea.Department)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var applications = await query
            .OrderByDescending(ea => ea.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (applications, totalCount);
    }

    public async Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> SearchApplicationsAsync(
        ApplicationStatus? status = null,
        string? departmentName = null,
        string? program = null,
        string? academicTerm = null,
        int? academicYear = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        _logger.LogDebug("Searching applications with criteria");

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _context.EnrollmentApplications
            .Include(ea => ea.Applicant)
            .Include(ea => ea.Department)
            .AsQueryable();

        // Apply filters
        if (status.HasValue)
        {
            query = query.Where(ea => ea.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(departmentName))
        {
            query = query.Where(ea => ea.DepartmentName.Contains(departmentName));
        }

        if (!string.IsNullOrWhiteSpace(program))
        {
            query = query.Where(ea => ea.Program.Contains(program));
        }

        if (!string.IsNullOrWhiteSpace(academicTerm))
        {
            query = query.Where(ea => ea.AcademicTerm == academicTerm);
        }

        if (academicYear.HasValue)
        {
            query = query.Where(ea => ea.AcademicYear == academicYear.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(ea =>
                ea.ApplicantName.ToLower().Contains(lowerSearchTerm) ||
                ea.Email.ToLower().Contains(lowerSearchTerm));
        }

        var totalCount = await query.CountAsync();

        var applications = await query
            .OrderByDescending(ea => ea.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (applications, totalCount);
    }

    public async Task<EnrollmentApplication> UpdateApplicationAsync(EnrollmentApplication application)
    {
        if (application == null)
            throw new ArgumentNullException(nameof(application));

        var existing = await GetApplicationByIdAsync(application.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"Application with ID {application.Id} not found");
        }

        ValidateApplication(application);

        // Update fields
        existing.ApplicantName = application.ApplicantName;
        existing.Email = application.Email;
        existing.PhoneNumber = application.PhoneNumber;
        existing.Program = application.Program;
        existing.DepartmentName = application.DepartmentName;
        existing.AcademicTerm = application.AcademicTerm;
        existing.AcademicYear = application.AcademicYear;
        existing.Notes = application.Notes;
        existing.ModifiedBy = "System";

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> ProcessAdmissionDecisionAsync(int applicationId, AdmissionDecision decision,
        string? reason = null, string? decisionMadeBy = null)
    {
        var application = await GetApplicationByIdAsync(applicationId);
        if (application == null) return false;

        _logger.LogInformation("Processing admission decision for application {ApplicationId}: {Decision}",
            applicationId, decision);

        application.Decision = decision;
        application.DecisionDate = DateTime.UtcNow;
        application.DecisionReason = reason;
        application.DecisionMadeBy = decisionMadeBy ?? "System";
        application.ModifiedBy = "System";

        // Update status based on decision
        application.Status = decision switch
        {
            AdmissionDecision.Admitted => ApplicationStatus.Approved,
            AdmissionDecision.ConditionallyAdmitted => ApplicationStatus.Approved,
            AdmissionDecision.Rejected => ApplicationStatus.Rejected,
            AdmissionDecision.Waitlisted => ApplicationStatus.OnHold,
            AdmissionDecision.Deferred => ApplicationStatus.OnHold,
            _ => application.Status
        };

        await _context.SaveChangesAsync();

        _logger.LogInformation("Admission decision processed successfully for application {ApplicationId}", applicationId);
        return true;
    }

    public async Task<bool> UpdateApplicationStatusAsync(int applicationId, ApplicationStatus newStatus, string? notes = null)
    {
        var application = await GetApplicationByIdAsync(applicationId);
        if (application == null) return false;

        _logger.LogInformation("Updating application {ApplicationId} status from {OldStatus} to {NewStatus}",
            applicationId, application.Status, newStatus);

        application.Status = newStatus;
        if (!string.IsNullOrWhiteSpace(notes))
        {
            application.Notes = string.IsNullOrWhiteSpace(application.Notes) ? notes : $"{application.Notes}\n{notes}";
        }
        application.ModifiedBy = "System";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ApplicationDocument> AddDocumentToApplicationAsync(int applicationId, ApplicationDocument document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        var application = await GetApplicationByIdAsync(applicationId);
        if (application == null)
            throw new InvalidOperationException($"Application with ID {applicationId} not found");

        document.ApplicationId = applicationId;
        document.UploadDate = DateTime.UtcNow;
        document.CreatedBy = "System";
        document.ModifiedBy = "System";

        _context.ApplicationDocuments.Add(document);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Added document {DocumentType} to application {ApplicationId}",
            document.DocumentType, applicationId);

        return document;
    }

    public async Task<IEnumerable<ApplicationDocument>> GetApplicationDocumentsAsync(int applicationId)
    {
        return await _context.ApplicationDocuments
            .Where(ad => ad.ApplicationId == applicationId)
            .OrderBy(ad => ad.DocumentType)
            .ToListAsync();
    }

    public async Task<bool> VerifyDocumentAsync(int documentId, string verifiedBy, string? notes = null)
    {
        var document = await _context.ApplicationDocuments.FindAsync(documentId);
        if (document == null) return false;

        document.IsVerified = true;
        document.VerificationDate = DateTime.UtcNow;
        document.VerifiedBy = verifiedBy;
        if (!string.IsNullOrWhiteSpace(notes))
        {
            document.Notes = notes;
        }
        document.ModifiedBy = "System";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AreAllRequiredDocumentsSubmittedAsync(int applicationId)
    {
        var documents = await GetApplicationDocumentsAsync(applicationId);
        var requiredDocuments = documents.Where(d => d.IsRequired);

        return requiredDocuments.All(d => d.IsVerified);
    }

    public async Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsByStatusAsync(
        ApplicationStatus status, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.EnrollmentApplications
            .Include(ea => ea.Applicant)
            .Include(ea => ea.Department)
            .Where(ea => ea.Status == status);

        var totalCount = await query.CountAsync();
        var applications = await query
            .OrderByDescending(ea => ea.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (applications, totalCount);
    }

    public async Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsRequiringReviewAsync(
        int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.EnrollmentApplications
            .Include(ea => ea.Applicant)
            .Include(ea => ea.Department)
            .Where(ea => ea.Status == ApplicationStatus.UnderReview ||
                        ea.Status == ApplicationStatus.IncompleteDocuments ||
                        (!ea.DocumentsComplete && ea.Status == ApplicationStatus.Submitted));

        var totalCount = await query.CountAsync();
        var applications = await query
            .OrderBy(ea => ea.Priority)
            .ThenByDescending(ea => ea.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (applications, totalCount);
    }

    public async Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsByDepartmentAsync(
        string departmentName, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.EnrollmentApplications
            .Include(ea => ea.Applicant)
            .Include(ea => ea.Department)
            .Where(ea => ea.DepartmentName == departmentName);

        var totalCount = await query.CountAsync();
        var applications = await query
            .OrderByDescending(ea => ea.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (applications, totalCount);
    }

    public async Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsByTermAsync(
        string academicTerm, int academicYear, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.EnrollmentApplications
            .Include(ea => ea.Applicant)
            .Include(ea => ea.Department)
            .Where(ea => ea.AcademicTerm == academicTerm && ea.AcademicYear == academicYear);

        var totalCount = await query.CountAsync();
        var applications = await query
            .OrderByDescending(ea => ea.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (applications, totalCount);
    }

    public async Task<bool> WithdrawApplicationAsync(int applicationId, string? reason = null)
    {
        var application = await GetApplicationByIdAsync(applicationId);
        if (application == null) return false;

        if (application.Status == ApplicationStatus.Approved || application.Status == ApplicationStatus.Rejected)
        {
            throw new InvalidOperationException("Cannot withdraw an application that has already been processed");
        }

        application.Status = ApplicationStatus.Withdrawn;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            application.Notes = string.IsNullOrWhiteSpace(application.Notes) ?
                $"Withdrawn: {reason}" : $"{application.Notes}\nWithdrawn: {reason}";
        }
        application.ModifiedBy = "System";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApplicationExistsAsync(int applicationId)
    {
        return await _context.EnrollmentApplications.AnyAsync(ea => ea.Id == applicationId);
    }

    public async Task<int> GetTotalApplicationCountAsync()
    {
        return await _context.EnrollmentApplications.CountAsync();
    }

    public async Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetApplicationsByPriorityAsync(
        ApplicationPriority priority, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.EnrollmentApplications
            .Include(ea => ea.Applicant)
            .Include(ea => ea.Department)
            .Where(ea => ea.Priority == priority);

        var totalCount = await query.CountAsync();
        var applications = await query
            .OrderByDescending(ea => ea.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (applications, totalCount);
    }

    private void ValidateApplication(EnrollmentApplication application)
    {
        if (string.IsNullOrWhiteSpace(application.ApplicantName))
            throw new ArgumentException("Applicant name is required", nameof(application.ApplicantName));

        if (string.IsNullOrWhiteSpace(application.Email))
            throw new ArgumentException("Email is required", nameof(application.Email));

        if (string.IsNullOrWhiteSpace(application.Program))
            throw new ArgumentException("Program is required", nameof(application.Program));

        if (string.IsNullOrWhiteSpace(application.DepartmentName))
            throw new ArgumentException("Department name is required", nameof(application.DepartmentName));

        // Validate email format (basic validation)
        if (!application.Email.Contains("@"))
            throw new ArgumentException("Invalid email format", nameof(application.Email));

        // Validate GPA if provided
        if (application.PreviousGPA.HasValue && (application.PreviousGPA < 0 || application.PreviousGPA > 4.0m))
            throw new ArgumentException("Previous GPA must be between 0.0 and 4.0", nameof(application.PreviousGPA));
    }

    public async Task<IEnumerable<EnrollmentApplication>> GetApplicationsByStudentAsync(
        int studentEmpNr, ApplicationStatus? status = null)
    {
        _logger.LogDebug("Getting applications for student {StudentEmpNr}", studentEmpNr);

        var query = _context.EnrollmentApplications
            .Include(ea => ea.Applicant)
            .Include(ea => ea.Department)
            .Include(ea => ea.Documents)
            .Where(ea => ea.ApplicantEmpNr == studentEmpNr);

        if (status.HasValue)
        {
            query = query.Where(ea => ea.Status == status.Value);
        }

        return await query
            .OrderByDescending(ea => ea.ApplicationDate)
            .ToListAsync();
    }
}