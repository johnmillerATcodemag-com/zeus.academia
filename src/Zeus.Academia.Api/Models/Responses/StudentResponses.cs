using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Api.Models.Responses;

/// <summary>
/// Response model for student details.
/// </summary>
public class StudentDetailsResponse
{
    /// <summary>
    /// The student's employee number (primary key).
    /// </summary>
    public int EmpNr { get; set; }

    /// <summary>
    /// The student's unique student ID.
    /// </summary>
    public string StudentId { get; set; } = string.Empty;

    /// <summary>
    /// The student's full name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The student's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The student's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The student's date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// The student's gender.
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// The student's nationality.
    /// </summary>
    public string? Nationality { get; set; }

    /// <summary>
    /// The student's address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// The name of the department the student belongs to.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// The degree code the student is pursuing.
    /// </summary>
    public string? DegreeCode { get; set; }

    /// <summary>
    /// The program the student is enrolled in.
    /// </summary>
    public string? Program { get; set; }

    /// <summary>
    /// The student's enrollment status.
    /// </summary>
    public EnrollmentStatus EnrollmentStatus { get; set; }

    /// <summary>
    /// The student's academic standing.
    /// </summary>
    public AcademicStanding AcademicStanding { get; set; }

    /// <summary>
    /// The student's current cumulative GPA.
    /// </summary>
    public decimal? CumulativeGPA { get; set; }

    /// <summary>
    /// The student's total credit hours completed.
    /// </summary>
    public decimal? CreditHoursCompleted { get; set; }

    /// <summary>
    /// The academic year the student started.
    /// </summary>
    public int? AcademicYear { get; set; }

    /// <summary>
    /// The student's expected graduation date.
    /// </summary>
    public DateTime? ExpectedGraduationDate { get; set; }

    /// <summary>
    /// The student's actual graduation date (if graduated).
    /// </summary>
    public DateTime? ActualGraduationDate { get; set; }

    /// <summary>
    /// The date of the student's last academic review.
    /// </summary>
    public DateTime? LastAcademicReviewDate { get; set; }

    /// <summary>
    /// Whether the student record is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// When the student record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// When the student record was last modified.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// Who created the student record.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Who last modified the student record.
    /// </summary>
    public string ModifiedBy { get; set; } = string.Empty;
}

/// <summary>
/// Summary response model for student lists.
/// </summary>
public class StudentSummaryResponse
{
    /// <summary>
    /// The student's employee number (primary key).
    /// </summary>
    public int EmpNr { get; set; }

    /// <summary>
    /// The student's unique student ID.
    /// </summary>
    public string StudentId { get; set; } = string.Empty;

    /// <summary>
    /// The student's full name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The student's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The name of the department the student belongs to.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// The program the student is enrolled in.
    /// </summary>
    public string? Program { get; set; }

    /// <summary>
    /// The student's enrollment status.
    /// </summary>
    public EnrollmentStatus EnrollmentStatus { get; set; }

    /// <summary>
    /// The student's academic standing.
    /// </summary>
    public AcademicStanding AcademicStanding { get; set; }

    /// <summary>
    /// The student's current cumulative GPA.
    /// </summary>
    public decimal? CumulativeGPA { get; set; }

    /// <summary>
    /// The academic year the student started.
    /// </summary>
    public int? AcademicYear { get; set; }
}

/// <summary>
/// Response model for enrollment applications.
/// </summary>
public class EnrollmentApplicationResponse
{
    /// <summary>
    /// The application ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The applicant's employee number.
    /// </summary>
    public int? ApplicantEmpNr { get; set; }

    /// <summary>
    /// The applicant's name.
    /// </summary>
    public string ApplicantName { get; set; } = string.Empty;

    /// <summary>
    /// The applicant's email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The program applied for.
    /// </summary>
    public string Program { get; set; } = string.Empty;

    /// <summary>
    /// The department name.
    /// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// The application date.
    /// </summary>
    public DateTime ApplicationDate { get; set; }

    /// <summary>
    /// The application status.
    /// </summary>
    public ApplicationStatus Status { get; set; }

    /// <summary>
    /// The application priority.
    /// </summary>
    public ApplicationPriority Priority { get; set; }

    /// <summary>
    /// The admission decision (if made).
    /// </summary>
    public AdmissionDecision? Decision { get; set; }

    /// <summary>
    /// The decision date (if made).
    /// </summary>
    public DateTime? DecisionDate { get; set; }

    /// <summary>
    /// The decision reason.
    /// </summary>
    public string? DecisionReason { get; set; }

    /// <summary>
    /// Who made the decision.
    /// </summary>
    public string? DecisionMadeBy { get; set; }

    /// <summary>
    /// Any conditional requirements.
    /// </summary>
    public string? ConditionalRequirements { get; set; }

    /// <summary>
    /// Application notes.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Response model for enrollment history.
/// </summary>
public class EnrollmentHistoryResponse
{
    /// <summary>
    /// The history entry ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The student's employee number.
    /// </summary>
    public int StudentEmpNr { get; set; }

    /// <summary>
    /// The enrollment event type.
    /// </summary>
    public EnrollmentEventType EventType { get; set; }

    /// <summary>
    /// The new enrollment status.
    /// </summary>
    public EnrollmentStatus NewStatus { get; set; }

    /// <summary>
    /// The previous enrollment status.
    /// </summary>
    public EnrollmentStatus? PreviousStatus { get; set; }

    /// <summary>
    /// The reason for the event.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Additional notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Who processed the event.
    /// </summary>
    public string? ProcessedBy { get; set; }

    /// <summary>
    /// The event date.
    /// </summary>
    public DateTime EventDate { get; set; }

    /// <summary>
    /// Associated application ID.
    /// </summary>
    public int? ApplicationId { get; set; }

    /// <summary>
    /// Department involved.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Program involved.
    /// </summary>
    public string? Program { get; set; }

    /// <summary>
    /// Academic term.
    /// </summary>
    public string? AcademicTerm { get; set; }

    /// <summary>
    /// Academic year.
    /// </summary>
    public int? AcademicYear { get; set; }
}

/// <summary>
/// Response model for file upload results.
/// </summary>
public class FileUploadResponse
{
    /// <summary>
    /// Whether the upload was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The uploaded file name.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// The file path or URL.
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// The file size in bytes.
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// Any error message if upload failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The upload timestamp.
    /// </summary>
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// Response model for student statistics.
/// </summary>
public class StudentStatisticsResponse
{
    /// <summary>
    /// Total number of students.
    /// </summary>
    public int TotalStudents { get; set; }

    /// <summary>
    /// Number of active students.
    /// </summary>
    public int ActiveStudents { get; set; }

    /// <summary>
    /// Number of enrolled students.
    /// </summary>
    public int EnrolledStudents { get; set; }

    /// <summary>
    /// Number of graduated students.
    /// </summary>
    public int GraduatedStudents { get; set; }

    /// <summary>
    /// Students by department.
    /// </summary>
    public Dictionary<string, int> StudentsByDepartment { get; set; } = new();

    /// <summary>
    /// Students by enrollment status.
    /// </summary>
    public Dictionary<EnrollmentStatus, int> StudentsByEnrollmentStatus { get; set; } = new();

    /// <summary>
    /// Students by academic standing.
    /// </summary>
    public Dictionary<AcademicStanding, int> StudentsByAcademicStanding { get; set; } = new();
}