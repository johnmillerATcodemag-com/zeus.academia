using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for managing students with comprehensive business logic
/// </summary>
public class StudentService : IStudentService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<StudentService> _logger;
    private readonly IEnrollmentApplicationService _enrollmentApplicationService;
    private readonly IEnrollmentHistoryService _enrollmentHistoryService;

    public StudentService(
        AcademiaDbContext context,
        ILogger<StudentService> logger,
        IEnrollmentApplicationService enrollmentApplicationService,
        IEnrollmentHistoryService enrollmentHistoryService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _enrollmentApplicationService = enrollmentApplicationService ?? throw new ArgumentNullException(nameof(enrollmentApplicationService));
        _enrollmentHistoryService = enrollmentHistoryService ?? throw new ArgumentNullException(nameof(enrollmentHistoryService));
    }

    public async Task<Student?> GetStudentByIdAsync(int studentId)
    {
        _logger.LogDebug("Getting student by ID: {StudentId}", studentId);

        return await _context.Students
            .Include(s => s.Department)
            .Include(s => s.Enrollments)
                .ThenInclude(se => se.Subject)
            .FirstOrDefaultAsync(s => s.EmpNr == studentId);
    }

    public async Task<Student?> GetStudentByStudentNumberAsync(string studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            _logger.LogWarning("GetStudentByStudentNumberAsync called with null or empty student number");
            return null;
        }

        _logger.LogDebug("Getting student by student number: {StudentNumber}", studentNumber);

        return await _context.Students
            .Include(s => s.Department)
            .Include(s => s.Enrollments)
                .ThenInclude(se => se.Subject)
            .FirstOrDefaultAsync(s => s.StudentId == studentNumber);
    }

    public async Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsAsync(int pageNumber = 1, int pageSize = 10)
    {
        _logger.LogDebug("Getting students - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Students
            .Include(s => s.Department)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var students = await query
            .OrderBy(s => s.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (students, totalCount);
    }

    public async Task<(IEnumerable<Student> Students, int TotalCount)> SearchStudentsAsync(
        string? searchTerm = null,
        string? departmentName = null,
        EnrollmentStatus? enrollmentStatus = null,
        AcademicStanding? academicStanding = null,
        string? program = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        _logger.LogDebug("Searching students with criteria");

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Students
            .Include(s => s.Department)
            .AsQueryable();

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(lowerSearchTerm) ||
                (s.StudentId != null && s.StudentId.ToLower().Contains(lowerSearchTerm)));
        }

        // Apply filters
        if (!string.IsNullOrWhiteSpace(departmentName))
        {
            query = query.Where(s => s.Department != null &&
                                   s.Department.Name.ToLower().Contains(departmentName.ToLower()));
        }

        if (enrollmentStatus.HasValue)
        {
            query = query.Where(s => s.EnrollmentStatus == enrollmentStatus.Value);
        }

        if (academicStanding.HasValue)
        {
            query = query.Where(s => s.AcademicStanding == academicStanding.Value);
        }

        if (!string.IsNullOrWhiteSpace(program))
        {
            query = query.Where(s => s.Program != null &&
                                   s.Program.ToLower().Contains(program.ToLower()));
        }

        var totalCount = await query.CountAsync();

        var students = await query
            .OrderBy(s => s.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (students, totalCount);
    }

    public async Task<Student> CreateStudentAsync(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student));

        _logger.LogInformation("Creating new student: {Name}", student.Name);

        ValidateStudentData(student);

        // Check for duplicate student ID
        if (!string.IsNullOrEmpty(student.StudentId))
        {
            var existingStudent = await GetStudentByStudentNumberAsync(student.StudentId);
            if (existingStudent != null)
            {
                throw new InvalidOperationException($"Student with ID {student.StudentId} already exists");
            }
        }

        // Set defaults
        student.CreatedBy = "System";
        student.ModifiedBy = "System";
        student.EnrollmentDate = student.EnrollmentDate ?? DateTime.UtcNow;

        if (student.EnrollmentStatus == default)
        {
            student.EnrollmentStatus = EnrollmentStatus.Applied;
        }

        if (student.AcademicStanding == default)
        {
            student.AcademicStanding = AcademicStanding.NewStudent;
        }

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully created student with ID: {StudentId}", student.EmpNr);
        return student;
    }

    public async Task<Student> UpdateStudentAsync(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student));

        var existing = await GetStudentByIdAsync(student.EmpNr);
        if (existing == null)
        {
            throw new InvalidOperationException($"Student with ID {student.EmpNr} not found");
        }

        ValidateStudentData(student);

        // Update basic fields
        existing.Name = student.Name;
        existing.StudentId = student.StudentId;
        existing.Program = student.Program;
        existing.EnrollmentStatus = student.EnrollmentStatus;
        existing.AcademicStanding = student.AcademicStanding;
        existing.CumulativeGPA = student.CumulativeGPA;
        existing.ModifiedBy = "System";

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteStudentAsync(int studentId)
    {
        var student = await GetStudentByIdAsync(studentId);
        if (student == null) return false;

        // Check for enrollments
        var hasEnrollments = await _context.StudentEnrollments
            .AnyAsync(se => se.StudentEmpNr == studentId);

        if (hasEnrollments)
        {
            throw new InvalidOperationException("Cannot delete student with existing enrollments");
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateEnrollmentStatusAsync(int studentId, EnrollmentStatus newStatus, string? notes = null)
    {
        var student = await GetStudentByIdAsync(studentId);
        if (student == null) return false;

        if (!IsValidEnrollmentStatusTransition(student.EnrollmentStatus, newStatus))
        {
            throw new InvalidOperationException($"Invalid status transition from {student.EnrollmentStatus} to {newStatus}");
        }

        student.EnrollmentStatus = newStatus;
        student.ModifiedBy = "System";

        if (newStatus == EnrollmentStatus.Graduated)
        {
            student.ActualGraduationDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAcademicStandingAsync(int studentId, AcademicStanding newStanding, string? notes = null)
    {
        var student = await GetStudentByIdAsync(studentId);
        if (student == null) return false;

        if (!IsValidAcademicStanding(student.CumulativeGPA, newStanding))
        {
            throw new InvalidOperationException($"Invalid academic standing {newStanding} for GPA {student.CumulativeGPA}");
        }

        student.AcademicStanding = newStanding;
        student.LastAcademicReviewDate = DateTime.UtcNow;
        student.ModifiedBy = "System";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal?> RecalculateGPAAsync(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.EmpNr == studentId);

        if (student == null) return null;

        // Simple GPA calculation - would need more sophisticated logic
        var completedEnrollments = student.Enrollments
            .Where(e => !string.IsNullOrEmpty(e.Grade) && e.Grade != "W")
            .ToList();

        if (!completedEnrollments.Any()) return null;

        // Basic GPA calculation
        var totalPoints = 0m;
        var totalCourses = 0;

        foreach (var enrollment in completedEnrollments)
        {
            var points = ConvertGradeToPoints(enrollment.Grade);
            if (points.HasValue)
            {
                totalPoints += points.Value;
                totalCourses++;
            }
        }

        var gpa = totalCourses > 0 ? totalPoints / totalCourses : 0;
        student.CumulativeGPA = Math.Round(gpa, 2);
        student.ModifiedBy = "System";

        await _context.SaveChangesAsync();
        return student.CumulativeGPA;
    }

    public async Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsByAcademicStandingAsync(
        AcademicStanding academicStanding, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Students.Where(s => s.AcademicStanding == academicStanding);
        var totalCount = await query.CountAsync();
        var students = await query.OrderBy(s => s.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (students, totalCount);
    }

    public async Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsByEnrollmentStatusAsync(
        EnrollmentStatus enrollmentStatus, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Students.Where(s => s.EnrollmentStatus == enrollmentStatus);
        var totalCount = await query.CountAsync();
        var students = await query.OrderBy(s => s.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (students, totalCount);
    }

    public async Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsByDepartmentAsync(
        string departmentName, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Students
            .Include(s => s.Department)
            .Where(s => s.Department != null && s.Department.Name.Contains(departmentName));
        var totalCount = await query.CountAsync();
        var students = await query.OrderBy(s => s.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (students, totalCount);
    }

    public bool IsValidEnrollmentStatusTransition(EnrollmentStatus currentStatus, EnrollmentStatus newStatus)
    {
        var validTransitions = new Dictionary<EnrollmentStatus, EnrollmentStatus[]>
        {
            [EnrollmentStatus.Applied] = new[] { EnrollmentStatus.Admitted, EnrollmentStatus.Dismissed },
            [EnrollmentStatus.Admitted] = new[] { EnrollmentStatus.Enrolled, EnrollmentStatus.Dismissed },
            [EnrollmentStatus.Enrolled] = new[] {
                EnrollmentStatus.Suspended, EnrollmentStatus.Withdrawn,
                EnrollmentStatus.Graduated, EnrollmentStatus.LeaveOfAbsence, EnrollmentStatus.Dismissed
            },
            [EnrollmentStatus.Suspended] = new[] { EnrollmentStatus.Enrolled, EnrollmentStatus.Dismissed },
            [EnrollmentStatus.LeaveOfAbsence] = new[] { EnrollmentStatus.Enrolled, EnrollmentStatus.Withdrawn },
            [EnrollmentStatus.Withdrawn] = new EnrollmentStatus[0],
            [EnrollmentStatus.Graduated] = new EnrollmentStatus[0],
            [EnrollmentStatus.Dismissed] = new EnrollmentStatus[0]
        };

        return validTransitions.ContainsKey(currentStatus) &&
               validTransitions[currentStatus].Contains(newStatus);
    }

    public bool IsValidAcademicStanding(decimal? currentGPA, AcademicStanding newStanding)
    {
        if (!currentGPA.HasValue)
        {
            return newStanding == AcademicStanding.NewStudent;
        }

        var gpa = currentGPA.Value;
        return newStanding switch
        {
            AcademicStanding.PresidentsListqualification => gpa >= 3.9m,
            AcademicStanding.DeansListqualification => gpa >= 3.5m,
            AcademicStanding.Good => gpa >= 2.0m,
            AcademicStanding.Warning => gpa >= 1.5m && gpa < 2.0m,
            AcademicStanding.Probation => gpa >= 1.0m && gpa < 2.0m,
            AcademicStanding.AcademicSuspension => gpa < 1.0m,
            AcademicStanding.AcademicDismissal => gpa < 1.0m,
            AcademicStanding.NewStudent => !currentGPA.HasValue,
            _ => false
        };
    }

    public async Task<bool> StudentExistsAsync(int studentId)
    {
        return await _context.Students.AnyAsync(s => s.EmpNr == studentId);
    }

    public async Task<int> GetTotalStudentCountAsync()
    {
        return await _context.Students.CountAsync();
    }

    public async Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsRequiringAcademicReviewAsync(
        int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Students
            .Where(s => (s.CumulativeGPA.HasValue && s.CumulativeGPA < 2.0m) ||
                       s.AcademicStanding == AcademicStanding.Probation ||
                       s.AcademicStanding == AcademicStanding.Warning ||
                       s.AcademicStanding == AcademicStanding.AcademicSuspension);

        var totalCount = await query.CountAsync();
        var students = await query
            .OrderBy(s => s.CumulativeGPA ?? 0)
            .ThenBy(s => s.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (students, totalCount);
    }

    private void ValidateStudentData(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.Name))
            throw new ArgumentException("Name is required", nameof(student.Name));

        if (student.CumulativeGPA.HasValue && (student.CumulativeGPA < 0 || student.CumulativeGPA > 4.0m))
            throw new ArgumentException("Cumulative GPA must be between 0.0 and 4.0");
    }

    private decimal? ConvertGradeToPoints(string? grade)
    {
        return grade?.ToUpper() switch
        {
            "A" => 4.0m,
            "A-" => 3.7m,
            "B+" => 3.3m,
            "B" => 3.0m,
            "B-" => 2.7m,
            "C+" => 2.3m,
            "C" => 2.0m,
            "C-" => 1.7m,
            "D+" => 1.3m,
            "D" => 1.0m,
            "F" => 0.0m,
            _ => null
        };
    }

    // Enrollment Management Methods

    public async Task<EnrollmentApplication> SubmitApplicationAsync(
        int studentId,
        string programCode,
        DateTime preferredStartDate,
        List<ApplicationDocument>? applicationDocuments = null)
    {
        _logger.LogInformation("Submitting enrollment application for student {StudentId}", studentId);

        var student = await GetStudentByIdAsync(studentId);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with ID {studentId} not found");
        }

        // Validate student can apply
        if (student.EnrollmentStatus == EnrollmentStatus.Enrolled)
        {
            throw new InvalidOperationException("Student is already enrolled");
        }

        if (student.EnrollmentStatus == EnrollmentStatus.Graduated)
        {
            throw new InvalidOperationException("Student has already graduated");
        }

        // Check for existing pending applications by searching all applications for this student
        var searchResults = await _enrollmentApplicationService.SearchApplicationsAsync(
            status: ApplicationStatus.Submitted,
            searchTerm: student.Name);

        // Filter to check for applications from this specific student by employee number
        var existingApplications = searchResults.Applications.Where(a => a.ApplicantEmpNr == studentId);
        if (existingApplications.Any())
        {
            throw new InvalidOperationException("Student already has a pending application");
        }

        // Create enrollment application
        var application = new EnrollmentApplication
        {
            ApplicantEmpNr = studentId,
            ApplicantName = student.Name,
            Email = student.Name.Replace(" ", ".").ToLower() + "@academia.edu",
            Program = programCode,
            DepartmentName = student.Department?.Name ?? "Unknown",
            ApplicationDate = DateTime.UtcNow,
            Status = ApplicationStatus.Submitted,
            Priority = ApplicationPriority.Normal,
            Notes = $"Application submitted for program: {programCode}",
            Documents = applicationDocuments ?? new List<ApplicationDocument>()
        };

        var createdApplication = await _enrollmentApplicationService.SubmitApplicationAsync(application);

        // Record enrollment history
        await _enrollmentHistoryService.RecordEnrollmentEventAsync(
            studentId,
            EnrollmentEventType.ApplicationSubmitted,
            student.EnrollmentStatus,
            null,
            $"Application submitted for program {programCode}",
            createdApplication.Id.ToString());

        // Update student status if currently Applied
        if (student.EnrollmentStatus == EnrollmentStatus.Applied)
        {
            await UpdateEnrollmentStatusAsync(studentId, EnrollmentStatus.Applied,
                "Application status updated - new application submitted");
        }

        _logger.LogInformation("Successfully submitted application {ApplicationId} for student {StudentId}",
            createdApplication.Id, studentId);

        return createdApplication;
    }

    public async Task<bool> ProcessAdmissionDecisionAsync(
        int applicationId,
        AdmissionDecision decision,
        string decisionReason,
        string decisionMadeBy,
        string? conditionalRequirements = null)
    {
        _logger.LogInformation("Processing admission decision for application {ApplicationId}: {Decision}",
            applicationId, decision);

        var application = await _enrollmentApplicationService.GetApplicationByIdAsync(applicationId);
        if (application == null)
        {
            throw new InvalidOperationException($"Application with ID {applicationId} not found");
        }

        var student = application.ApplicantEmpNr.HasValue
            ? await GetStudentByIdAsync(application.ApplicantEmpNr.Value)
            : null;

        if (student == null)
        {
            throw new InvalidOperationException($"Student with ID {application.ApplicantEmpNr} not found");
        }

        // Process the admission decision
        var success = await _enrollmentApplicationService.ProcessAdmissionDecisionAsync(
            applicationId, decision, decisionReason, decisionMadeBy);

        if (!success)
        {
            return false;
        }

        // Update student enrollment status based on decision
        var newStatus = decision switch
        {
            AdmissionDecision.Admitted => EnrollmentStatus.Admitted,
            AdmissionDecision.Rejected => EnrollmentStatus.Dismissed,
            AdmissionDecision.Waitlisted => student.EnrollmentStatus, // Keep current status
            AdmissionDecision.ConditionallyAdmitted => EnrollmentStatus.Admitted,
            _ => student.EnrollmentStatus
        };

        if (newStatus != student.EnrollmentStatus && application.ApplicantEmpNr.HasValue)
        {
            await UpdateEnrollmentStatusAsync(application.ApplicantEmpNr.Value, newStatus,
                $"Status updated based on admission decision: {decision}");
        }

        // Record enrollment history
        var eventType = decision switch
        {
            AdmissionDecision.Admitted => EnrollmentEventType.AdmissionDecision,
            AdmissionDecision.Rejected => EnrollmentEventType.AdmissionDecision,
            AdmissionDecision.Waitlisted => EnrollmentEventType.AdmissionDecision,
            AdmissionDecision.ConditionallyAdmitted => EnrollmentEventType.AdmissionDecision,
            _ => EnrollmentEventType.AdmissionDecision
        };

        if (application.ApplicantEmpNr.HasValue)
        {
            await _enrollmentHistoryService.RecordEnrollmentEventAsync(
                application.ApplicantEmpNr.Value,
                eventType,
                newStatus,
                student.EnrollmentStatus,
                $"Admission decision: {decision} - {decisionReason}",
                applicationId.ToString(),
                decisionMadeBy);
        }

        _logger.LogInformation("Successfully processed admission decision {Decision} for application {ApplicationId}",
            decision, applicationId);

        return true;
    }

    public async Task<bool> ProcessEnrollmentAsync(
        int applicationId,
        DateTime enrollmentDate,
        int academicTermId,
        string? notes = null)
    {
        _logger.LogInformation("Processing enrollment for application {ApplicationId}", applicationId);

        var application = await _enrollmentApplicationService.GetApplicationByIdAsync(applicationId);
        if (application == null)
        {
            throw new InvalidOperationException($"Application with ID {applicationId} not found");
        }

        if (application.Decision != AdmissionDecision.Admitted &&
            application.Decision != AdmissionDecision.ConditionallyAdmitted)
        {
            throw new InvalidOperationException("Cannot enroll student without accepted admission");
        }

        if (!application.ApplicantEmpNr.HasValue)
        {
            throw new InvalidOperationException("Application must have a valid student ID");
        }

        var student = await GetStudentByIdAsync(application.ApplicantEmpNr.Value);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with ID {application.ApplicantEmpNr} not found");
        }

        if (student.EnrollmentStatus != EnrollmentStatus.Admitted)
        {
            throw new InvalidOperationException("Student must be in Admitted status to enroll");
        }

        // Update student enrollment information
        student.EnrollmentStatus = EnrollmentStatus.Enrolled;
        student.EnrollmentDate = enrollmentDate;
        student.Program = application.Program;
        student.ModifiedBy = "System";

        await _context.SaveChangesAsync();

        // Update application status
        await _enrollmentApplicationService.UpdateApplicationStatusAsync(
            applicationId, ApplicationStatus.Approved, notes ?? "Student successfully enrolled");

        // Record enrollment history
        await _enrollmentHistoryService.RecordEnrollmentEventAsync(
            application.ApplicantEmpNr.Value,
            EnrollmentEventType.Enrolled,
            EnrollmentStatus.Enrolled,
            EnrollmentStatus.Admitted,
            $"Student enrolled in program {application.Program} for term {academicTermId}",
            notes,
            "System",
            applicationId);

        _logger.LogInformation("Successfully processed enrollment for student {StudentId} in application {ApplicationId}",
            application.ApplicantEmpNr.Value, applicationId);

        return true;
    }

    public async Task<IEnumerable<EnrollmentApplication>> GetStudentApplicationsAsync(
        int studentId,
        ApplicationStatus? status = null)
    {
        _logger.LogDebug("Getting applications for student {StudentId}", studentId);

        return await _enrollmentApplicationService.GetApplicationsByStudentAsync(studentId, status);
    }

    public async Task<IEnumerable<EnrollmentHistory>> GetStudentEnrollmentHistoryAsync(
        int studentId,
        bool includeDetails = false)
    {
        _logger.LogDebug("Getting enrollment history for student {StudentId}", studentId);

        var (history, _) = await _enrollmentHistoryService.GetStudentEnrollmentHistoryAsync(studentId);
        return history;
    }

    public async Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetPendingApplicationsAsync(
        int pageNumber = 1,
        int pageSize = 10)
    {
        _logger.LogDebug("Getting pending applications - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        return await _enrollmentApplicationService.GetApplicationsByStatusAsync(
            ApplicationStatus.Submitted, pageNumber, pageSize);
    }
}