using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a student's enrollment application
/// </summary>
public class EnrollmentApplication : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the application.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the applicant's employee number (if they're already in the system).
    /// </summary>
    public int? ApplicantEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the applicant's full name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ApplicantName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's email address.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the applicant's phone number.
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the program the student is applying for.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Program { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the department name.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the academic term for which the student is applying.
    /// </summary>
    [MaxLength(20)]
    public string? AcademicTerm { get; set; }

    /// <summary>
    /// Gets or sets the academic year for application.
    /// </summary>
    public int? AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the application submission date.
    /// </summary>
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the current status of the application.
    /// </summary>
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;

    /// <summary>
    /// Gets or sets the admission decision.
    /// </summary>
    public AdmissionDecision? Decision { get; set; }

    /// <summary>
    /// Gets or sets the date when the decision was made.
    /// </summary>
    public DateTime? DecisionDate { get; set; }

    /// <summary>
    /// Gets or sets the reason or notes for the decision.
    /// </summary>
    [MaxLength(1000)]
    public string? DecisionReason { get; set; }

    /// <summary>
    /// Gets or sets who made the admission decision.
    /// </summary>
    [MaxLength(100)]
    public string? DecisionMadeBy { get; set; }

    /// <summary>
    /// Gets or sets the priority level of the application.
    /// </summary>
    public ApplicationPriority Priority { get; set; } = ApplicationPriority.Normal;

    /// <summary>
    /// Gets or sets the expected enrollment date if admitted.
    /// </summary>
    public DateTime? ExpectedEnrollmentDate { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the application.
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether all required documents have been submitted.
    /// </summary>
    public bool DocumentsComplete { get; set; } = false;

    /// <summary>
    /// Gets or sets the list of submitted document types.
    /// </summary>
    [MaxLength(500)]
    public string? SubmittedDocuments { get; set; }

    /// <summary>
    /// Gets or sets the GPA from previous education (if applicable).
    /// </summary>
    public decimal? PreviousGPA { get; set; }

    /// <summary>
    /// Gets or sets the previous institution name.
    /// </summary>
    [MaxLength(200)]
    public string? PreviousInstitution { get; set; }

    /// <summary>
    /// Gets or sets the graduation date from previous institution.
    /// </summary>
    public DateTime? PreviousGraduationDate { get; set; }

    /// <summary>
    /// Gets or sets whether the applicant requires financial aid.
    /// </summary>
    public bool RequiresFinancialAid { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the applicant is an international student.
    /// </summary>
    public bool IsInternationalStudent { get; set; } = false;

    /// <summary>
    /// Navigation property to the applicant (if they're already a student).
    /// </summary>
    public virtual Student? Applicant { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Navigation property to application documents.
    /// </summary>
    public virtual ICollection<ApplicationDocument> Documents { get; set; } = new List<ApplicationDocument>();

    /// <summary>
    /// Navigation property to enrollment history if application is processed.
    /// </summary>
    public virtual ICollection<EnrollmentHistory> EnrollmentHistory { get; set; } = new List<EnrollmentHistory>();
}