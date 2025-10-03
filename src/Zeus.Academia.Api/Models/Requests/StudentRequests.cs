using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Api.Models.Requests;

/// <summary>
/// Request model for creating a new student.
/// </summary>
public class CreateStudentRequest
{
    /// <summary>
    /// The student's full name - required and must be between 2-100 characters.
    /// </summary>
    [Required(ErrorMessage = "Student name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Student name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The student's unique student ID - required and must be between 3-20 characters.
    /// </summary>
    [Required(ErrorMessage = "Student ID is required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Student ID must be between 3 and 20 characters")]
    public string StudentId { get; set; } = string.Empty;

    /// <summary>
    /// The student's email address - required and must be valid format.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number - must be in valid format if provided.
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The student's date of birth.
    /// </summary>
    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// The student's gender.
    /// </summary>
    [StringLength(20, ErrorMessage = "Gender must not exceed 20 characters")]
    public string? Gender { get; set; }

    /// <summary>
    /// The student's nationality.
    /// </summary>
    [StringLength(50, ErrorMessage = "Nationality must not exceed 50 characters")]
    public string? Nationality { get; set; }

    /// <summary>
    /// The student's address.
    /// </summary>
    [StringLength(500, ErrorMessage = "Address must not exceed 500 characters")]
    public string? Address { get; set; }

    /// <summary>
    /// The name of the department the student belongs to.
    /// </summary>
    [StringLength(100, ErrorMessage = "Department name must not exceed 100 characters")]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// The degree code the student is pursuing.
    /// </summary>
    [StringLength(10, ErrorMessage = "Degree code must not exceed 10 characters")]
    public string? DegreeCode { get; set; }

    /// <summary>
    /// The program the student is enrolled in.
    /// </summary>
    [StringLength(100, ErrorMessage = "Program must not exceed 100 characters")]
    public string? Program { get; set; }

    /// <summary>
    /// The student's enrollment status.
    /// </summary>
    public EnrollmentStatus EnrollmentStatus { get; set; } = EnrollmentStatus.Applied;

    /// <summary>
    /// The student's academic standing.
    /// </summary>
    public AcademicStanding AcademicStanding { get; set; } = AcademicStanding.NewStudent;

    /// <summary>
    /// The academic year the student started.
    /// </summary>
    [Range(1900, 3000, ErrorMessage = "Academic year must be between 1900 and 3000")]
    public int? AcademicYear { get; set; }

    /// <summary>
    /// The student's expected graduation date.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? ExpectedGraduationDate { get; set; }

    /// <summary>
    /// The department ID the student belongs to.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// The student's current class/year.
    /// </summary>
    [StringLength(20, ErrorMessage = "Class must not exceed 20 characters")]
    public string? Class { get; set; }

    /// <summary>
    /// The student's GPA.
    /// </summary>
    [Range(0.0, 4.0, ErrorMessage = "GPA must be between 0.0 and 4.0")]
    public decimal? GPA { get; set; }
}

/// <summary>
/// Request model for updating an existing student.
/// </summary>
public class UpdateStudentRequest
{
    /// <summary>
    /// The student's full name - required and must be between 2-100 characters.
    /// </summary>
    [Required(ErrorMessage = "Student name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Student name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The student's email address - required and must be valid format.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number - must be in valid format if provided.
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The student's date of birth.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// The student's gender.
    /// </summary>
    [StringLength(20, ErrorMessage = "Gender must not exceed 20 characters")]
    public string? Gender { get; set; }

    /// <summary>
    /// The student's nationality.
    /// </summary>
    [StringLength(50, ErrorMessage = "Nationality must not exceed 50 characters")]
    public string? Nationality { get; set; }

    /// <summary>
    /// The student's address.
    /// </summary>
    [StringLength(500, ErrorMessage = "Address must not exceed 500 characters")]
    public string? Address { get; set; }

    /// <summary>
    /// The name of the department the student belongs to.
    /// </summary>
    [StringLength(100, ErrorMessage = "Department name must not exceed 100 characters")]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// The degree code the student is pursuing.
    /// </summary>
    [StringLength(10, ErrorMessage = "Degree code must not exceed 10 characters")]
    public string? DegreeCode { get; set; }

    /// <summary>
    /// The program the student is enrolled in.
    /// </summary>
    [StringLength(100, ErrorMessage = "Program must not exceed 100 characters")]
    public string? Program { get; set; }

    /// <summary>
    /// The student's expected graduation date.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? ExpectedGraduationDate { get; set; }

    /// <summary>
    /// The department ID the student belongs to.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// The student's current class/year.
    /// </summary>
    [StringLength(20, ErrorMessage = "Class must not exceed 20 characters")]
    public string? Class { get; set; }

    /// <summary>
    /// The student's GPA.
    /// </summary>
    [Range(0.0, 4.0, ErrorMessage = "GPA must be between 0.0 and 4.0")]
    public decimal? GPA { get; set; }
}

/// <summary>
/// Request model for searching students with various criteria.
/// </summary>
public class StudentSearchRequest : PaginationParameters
{
    /// <summary>
    /// General search term (searches name, email, student ID).
    /// </summary>
    [StringLength(100, ErrorMessage = "Search term must not exceed 100 characters")]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by department name.
    /// </summary>
    [StringLength(100, ErrorMessage = "Department name must not exceed 100 characters")]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Filter by enrollment status.
    /// </summary>
    public EnrollmentStatus? EnrollmentStatus { get; set; }

    /// <summary>
    /// Filter by academic standing.
    /// </summary>
    public AcademicStanding? AcademicStanding { get; set; }

    /// <summary>
    /// Filter by program.
    /// </summary>
    [StringLength(100, ErrorMessage = "Program must not exceed 100 characters")]
    public string? Program { get; set; }

    /// <summary>
    /// Filter by academic year.
    /// </summary>
    [Range(1900, 3000, ErrorMessage = "Academic year must be between 1900 and 3000")]
    public int? AcademicYear { get; set; }

    /// <summary>
    /// Filter by department ID.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Filter by minimum GPA.
    /// </summary>
    [Range(0.0, 4.0, ErrorMessage = "Min GPA must be between 0.0 and 4.0")]
    public decimal? MinGPA { get; set; }

    /// <summary>
    /// Filter by maximum GPA.
    /// </summary>
    [Range(0.0, 4.0, ErrorMessage = "Max GPA must be between 0.0 and 4.0")]
    public decimal? MaxGPA { get; set; }

    /// <summary>
    /// Filter by active status.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Page number for pagination (inherited but included for clarity).
    /// </summary>
    public int PageNumber { get; set; } = 1;
}

/// <summary>
/// Request model for updating student enrollment status.
/// </summary>
public class UpdateEnrollmentStatusRequest
{
    /// <summary>
    /// The new enrollment status.
    /// </summary>
    [Required(ErrorMessage = "Enrollment status is required")]
    public EnrollmentStatus EnrollmentStatus { get; set; }

    /// <summary>
    /// Optional notes about the status change.
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
    public string? Notes { get; set; }

    /// <summary>
    /// The new enrollment status.
    /// </summary>
    [Required(ErrorMessage = "New status is required")]
    public EnrollmentStatus NewStatus { get; set; }

    /// <summary>
    /// Reason for the status change.
    /// </summary>
    [StringLength(500, ErrorMessage = "Reason must not exceed 500 characters")]
    public string? Reason { get; set; }

    /// <summary>
    /// Effective date of the status change.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? EffectiveDate { get; set; }
}

/// <summary>
/// Request model for updating student academic standing.
/// </summary>
public class UpdateAcademicStandingRequest
{
    /// <summary>
    /// The new academic standing.
    /// </summary>
    [Required(ErrorMessage = "Academic standing is required")]
    public AcademicStanding AcademicStanding { get; set; }

    /// <summary>
    /// Optional notes about the standing change.
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
    public string? Notes { get; set; }

    /// <summary>
    /// The new academic standing.
    /// </summary>
    [Required(ErrorMessage = "New standing is required")]
    public AcademicStanding NewStanding { get; set; }

    /// <summary>
    /// Reason for the standing change.
    /// </summary>
    [StringLength(500, ErrorMessage = "Reason must not exceed 500 characters")]
    public string? Reason { get; set; }

    /// <summary>
    /// Review date for the standing change.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? ReviewDate { get; set; }
}

/// <summary>
/// Request model for submitting an enrollment application.
/// </summary>
public class SubmitApplicationRequest
{
    /// <summary>
    /// The program code to apply for.
    /// </summary>
    [Required(ErrorMessage = "Program code is required")]
    [StringLength(20, ErrorMessage = "Program code must not exceed 20 characters")]
    public string ProgramCode { get; set; } = string.Empty;

    /// <summary>
    /// The preferred start date for the program.
    /// </summary>
    [Required(ErrorMessage = "Preferred start date is required")]
    [DataType(DataType.Date)]
    public DateTime PreferredStartDate { get; set; }

    /// <summary>
    /// Optional application documents.
    /// </summary>
    public List<ApplicationDocumentRequest>? Documents { get; set; }

    /// <summary>
    /// Applicant employee number.
    /// </summary>
    [Required(ErrorMessage = "Applicant employee number is required")]
    [StringLength(20, ErrorMessage = "Employee number must not exceed 20 characters")]
    public string ApplicantEmpNr { get; set; } = string.Empty;

    /// <summary>
    /// Program to apply for.
    /// </summary>
    [Required(ErrorMessage = "Program is required")]
    [StringLength(100, ErrorMessage = "Program must not exceed 100 characters")]
    public string Program { get; set; } = string.Empty;

    /// <summary>
    /// Department ID for the application.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Application date.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? ApplicationDate { get; set; }

    /// <summary>
    /// Application priority.
    /// </summary>
    [StringLength(20, ErrorMessage = "Priority must not exceed 20 characters")]
    public string? Priority { get; set; }

    /// <summary>
    /// Personal statement.
    /// </summary>
    [StringLength(2000, ErrorMessage = "Personal statement must not exceed 2000 characters")]
    public string? PersonalStatement { get; set; }

    /// <summary>
    /// Academic history.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Academic history must not exceed 1000 characters")]
    public string? AcademicHistory { get; set; }

    /// <summary>
    /// References.
    /// </summary>
    [StringLength(1000, ErrorMessage = "References must not exceed 1000 characters")]
    public string? References { get; set; }

    /// <summary>
    /// Additional documents.
    /// </summary>
    [StringLength(500, ErrorMessage = "Additional documents must not exceed 500 characters")]
    public string? AdditionalDocuments { get; set; }
}

/// <summary>
/// Request model for application documents.
/// </summary>
public class ApplicationDocumentRequest
{
    /// <summary>
    /// The document type.
    /// </summary>
    [Required(ErrorMessage = "Document type is required")]
    [StringLength(50, ErrorMessage = "Document type must not exceed 50 characters")]
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// The document name or title.
    /// </summary>
    [Required(ErrorMessage = "Document name is required")]
    [StringLength(255, ErrorMessage = "Document name must not exceed 255 characters")]
    public string DocumentName { get; set; } = string.Empty;

    /// <summary>
    /// The file path or URL to the document.
    /// </summary>
    [StringLength(500, ErrorMessage = "File path must not exceed 500 characters")]
    public string? FilePath { get; set; }
}

/// <summary>
/// Request model for processing admission decisions.
/// </summary>
public class ProcessAdmissionDecisionRequest
{
    /// <summary>
    /// The admission decision.
    /// </summary>
    [Required(ErrorMessage = "Admission decision is required")]
    public AdmissionDecision Decision { get; set; }

    /// <summary>
    /// The reason for the decision.
    /// </summary>
    [Required(ErrorMessage = "Decision reason is required")]
    [StringLength(500, ErrorMessage = "Decision reason must not exceed 500 characters")]
    public string DecisionReason { get; set; } = string.Empty;

    /// <summary>
    /// The person making the decision.
    /// </summary>
    [Required(ErrorMessage = "Decision maker is required")]
    [StringLength(100, ErrorMessage = "Decision maker must not exceed 100 characters")]
    public string DecisionMadeBy { get; set; } = string.Empty;

    /// <summary>
    /// Optional conditional requirements for conditional admission.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Conditional requirements must not exceed 1000 characters")]
    public string? ConditionalRequirements { get; set; }

    /// <summary>
    /// Decision date.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? DecisionDate { get; set; }
}

/// <summary>
/// Request model for processing enrollment.
/// </summary>
public class ProcessEnrollmentRequest
{
    /// <summary>
    /// The enrollment date.
    /// </summary>
    [Required(ErrorMessage = "Enrollment date is required")]
    [DataType(DataType.Date)]
    public DateTime EnrollmentDate { get; set; }

    /// <summary>
    /// The academic term ID for enrollment.
    /// </summary>
    [Required(ErrorMessage = "Academic term ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Academic term ID must be a positive number")]
    public int AcademicTermId { get; set; }

    /// <summary>
    /// Optional enrollment notes.
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
    public string? Notes { get; set; }

    /// <summary>
    /// Student ID for enrollment.
    /// </summary>
    [Required(ErrorMessage = "Student ID is required")]
    [StringLength(20, ErrorMessage = "Student ID must not exceed 20 characters")]
    public string StudentId { get; set; } = string.Empty;

    /// <summary>
    /// Academic term for enrollment.
    /// </summary>
    [StringLength(20, ErrorMessage = "Academic term must not exceed 20 characters")]
    public string? AcademicTerm { get; set; }

    /// <summary>
    /// Academic year for enrollment.
    /// </summary>
    [Range(1900, 3000, ErrorMessage = "Academic year must be between 1900 and 3000")]
    public int? AcademicYear { get; set; }
}