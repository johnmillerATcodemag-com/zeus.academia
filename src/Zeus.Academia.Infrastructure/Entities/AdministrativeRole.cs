using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing administrative roles in the university hierarchy.
/// Defines positions like Dean, Associate Dean, Vice Provost, etc.
/// </summary>
public class AdministrativeRole : BaseEntity
{
    /// <summary>
    /// Gets or sets the role code (unique identifier).
    /// </summary>
    [Key]
    [Required]
    [StringLength(20)]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role title.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string RoleTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role description.
    /// </summary>
    [StringLength(500)]
    public string? RoleDescription { get; set; }

    /// <summary>
    /// Gets or sets the role level in the administrative hierarchy.
    /// </summary>
    [Range(1, 20)]
    public int HierarchyLevel { get; set; } = 5; // 1=President, 5=Dean, 10=Department Chair, etc.

    /// <summary>
    /// Gets or sets the role category (Academic, Administrative, Student Affairs, etc.).
    /// </summary>
    [Required]
    [StringLength(50)]
    public string RoleCategory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scope of authority (University, College, Department, Program).
    /// </summary>
    [Required]
    [StringLength(20)]
    public string AuthorityScope { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this role requires faculty status.
    /// </summary>
    public bool RequiresFacultyStatus { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum rank required for this role.
    /// </summary>
    [StringLength(20)]
    public string? MinimumRankRequired { get; set; }

    /// <summary>
    /// Gets or sets the minimum years of experience required.
    /// </summary>
    [Range(0, 50)]
    public int? MinimumYearsExperience { get; set; }

    /// <summary>
    /// Gets or sets the typical term length in years.
    /// </summary>
    [Range(1, 10)]
    public int TypicalTermLength { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum number of consecutive terms allowed.
    /// </summary>
    [Range(1, 10)]
    public int? MaxConsecutiveTerms { get; set; }

    /// <summary>
    /// Gets or sets whether this role includes tenure.
    /// </summary>
    public bool IncludesTenure { get; set; } = false;

    /// <summary>
    /// Gets or sets the base salary range minimum.
    /// </summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal? BaseSalaryRangeMin { get; set; }

    /// <summary>
    /// Gets or sets the base salary range maximum.
    /// </summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal? BaseSalaryRangeMax { get; set; }

    /// <summary>
    /// Gets or sets the administrative stipend or supplement.
    /// </summary>
    [Column(TypeName = "decimal(12,2)")]
    public decimal? AdministrativeStipend { get; set; }

    /// <summary>
    /// Gets or sets whether the role includes course release time.
    /// </summary>
    public bool IncludesCourseRelease { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of courses released per term.
    /// </summary>
    [Range(0, 6)]
    public int? CourseReleaseCount { get; set; }

    /// <summary>
    /// Gets or sets whether the role includes administrative support staff.
    /// </summary>
    public bool IncludesAdminSupport { get; set; } = true;

    /// <summary>
    /// Gets or sets additional benefits or perquisites.
    /// </summary>
    [StringLength(1000)]
    public string? AdditionalBenefits { get; set; }

    /// <summary>
    /// Gets or sets the key responsibilities for this role.
    /// </summary>
    [StringLength(2000)]
    public string? KeyResponsibilities { get; set; }

    /// <summary>
    /// Gets or sets the reporting structure (who this role reports to).
    /// </summary>
    [StringLength(100)]
    public string? ReportsTo { get; set; }

    /// <summary>
    /// Gets or sets roles that report to this position.
    /// </summary>
    [StringLength(500)]
    public string? SupervisesRoles { get; set; }

    /// <summary>
    /// Gets or sets the budget authority level.
    /// </summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal? BudgetAuthority { get; set; }

    /// <summary>
    /// Gets or sets the hiring authority level.
    /// </summary>
    [StringLength(100)]
    public string? HiringAuthority { get; set; }

    /// <summary>
    /// Gets or sets whether this role is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date this role was established.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? EstablishedDate { get; set; }

    /// <summary>
    /// Gets or sets any special qualifications required for this role.
    /// </summary>
    [StringLength(1000)]
    public string? SpecialQualifications { get; set; }

    /// <summary>
    /// Navigation property for assignments to this role.
    /// </summary>
    public virtual ICollection<AdministrativeAssignment> Assignments { get; set; } = new List<AdministrativeAssignment>();

    // Computed properties
    /// <summary>
    /// Gets whether this is a senior administrative role (level 1-5).
    /// </summary>
    [NotMapped]
    public bool IsSeniorRole => HierarchyLevel <= 5;

    /// <summary>
    /// Gets whether this is a middle management role (level 6-10).
    /// </summary>
    [NotMapped]
    public bool IsMiddleManagementRole => HierarchyLevel > 5 && HierarchyLevel <= 10;

    /// <summary>
    /// Gets the salary range as a formatted string.
    /// </summary>
    [NotMapped]
    public string SalaryRangeDisplay => BaseSalaryRangeMin.HasValue && BaseSalaryRangeMax.HasValue
        ? $"${BaseSalaryRangeMin:N0} - ${BaseSalaryRangeMax:N0}"
        : "Not specified";
}

/// <summary>
/// Entity representing assignments of faculty to administrative roles.
/// Manages the appointment and tenure of administrators in various positions.
/// </summary>
public class AdministrativeAssignment : BaseEntity
{
    /// <summary>
    /// Gets or sets the role code being assigned.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee number of the assignee.
    /// </summary>
    [Required]
    public int AssigneeEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the assignment start date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime AssignmentStartDate { get; set; }

    /// <summary>
    /// Gets or sets the assignment end date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? AssignmentEndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this is the current active assignment.
    /// </summary>
    [Required]
    public bool IsCurrent { get; set; } = true;

    /// <summary>
    /// Gets or sets the appointment type (Permanent, Interim, Acting, Temporary).
    /// </summary>
    [Required]
    [StringLength(20)]
    public string AppointmentType { get; set; } = "Permanent";

    /// <summary>
    /// Gets or sets the appointment status (Active, Inactive, On Leave, Resigned).
    /// </summary>
    [Required]
    [StringLength(20)]
    public string AppointmentStatus { get; set; } = "Active";

    /// <summary>
    /// Gets or sets the term number for this assignee in this role.
    /// </summary>
    [Range(1, 20)]
    public int TermNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the actual term length in years for this assignment.
    /// </summary>
    [Range(1, 10)]
    public int TermLengthYears { get; set; } = 5;

    /// <summary>
    /// Gets or sets who made this appointment.
    /// </summary>
    [StringLength(100)]
    public string? AppointedBy { get; set; }

    /// <summary>
    /// Gets or sets the appointment method (Board Action, Executive Decision, Search Committee).
    /// </summary>
    [StringLength(50)]
    public string? AppointmentMethod { get; set; }

    /// <summary>
    /// Gets or sets the date the appointment was announced.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? AnnouncementDate { get; set; }

    /// <summary>
    /// Gets or sets the confirmation or ratification date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? ConfirmationDate { get; set; }

    /// <summary>
    /// Gets or sets the actual salary for this assignment.
    /// </summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal? ActualSalary { get; set; }

    /// <summary>
    /// Gets or sets the administrative stipend for this assignment.
    /// </summary>
    [Column(TypeName = "decimal(12,2)")]
    public decimal? AdministrativeStipend { get; set; }

    /// <summary>
    /// Gets or sets the actual course release count for this assignment.
    /// </summary>
    [Range(0, 6)]
    public int? ActualCourseReleaseCount { get; set; }

    /// <summary>
    /// Gets or sets the reporting unit or college for this assignment.
    /// </summary>
    [StringLength(100)]
    public string? ReportingUnit { get; set; }

    /// <summary>
    /// Gets or sets the office location for this assignment.
    /// </summary>
    [StringLength(100)]
    public string? OfficeLocation { get; set; }

    /// <summary>
    /// Gets or sets the direct phone line for this role.
    /// </summary>
    [StringLength(20)]
    public string? DirectPhoneLine { get; set; }

    /// <summary>
    /// Gets or sets the administrative email for this role.
    /// </summary>
    [StringLength(100)]
    public string? AdministrativeEmail { get; set; }

    /// <summary>
    /// Gets or sets whether the assignee has an executive assistant.
    /// </summary>
    public bool HasExecutiveAssistant { get; set; } = false;

    /// <summary>
    /// Gets or sets the name of the executive assistant.
    /// </summary>
    [StringLength(100)]
    public string? ExecutiveAssistantName { get; set; }

    /// <summary>
    /// Gets or sets specific goals or initiatives for this assignment.
    /// </summary>
    [StringLength(2000)]
    public string? AssignmentGoals { get; set; }

    /// <summary>
    /// Gets or sets key performance indicators for this role.
    /// </summary>
    [StringLength(1000)]
    public string? KeyPerformanceIndicators { get; set; }

    /// <summary>
    /// Gets or sets the last performance review date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? LastPerformanceReview { get; set; }

    /// <summary>
    /// Gets or sets the next scheduled performance review date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? NextPerformanceReview { get; set; }

    /// <summary>
    /// Gets or sets performance evaluation notes.
    /// </summary>
    [StringLength(2000)]
    public string? PerformanceNotes { get; set; }

    /// <summary>
    /// Gets or sets the reason for assignment change or termination.
    /// </summary>
    [StringLength(100)]
    public string? ChangeReason { get; set; }

    /// <summary>
    /// Gets or sets succession planning notes.
    /// </summary>
    [StringLength(1000)]
    public string? SuccessionPlan { get; set; }

    /// <summary>
    /// Gets or sets transition notes for incoming/outgoing administrators.
    /// </summary>
    [StringLength(2000)]
    public string? TransitionNotes { get; set; }

    /// <summary>
    /// Navigation property to the administrative role.
    /// </summary>
    public virtual AdministrativeRole Role { get; set; } = null!;

    /// <summary>
    /// Navigation property to the assigned faculty member.
    /// </summary>
    public virtual Academic Assignee { get; set; } = null!;

    // Computed properties
    /// <summary>
    /// Gets whether the assignment is currently active.
    /// </summary>
    [NotMapped]
    public bool IsActive => IsCurrent && AppointmentStatus == "Active"
        && (AssignmentEndDate == null || AssignmentEndDate > DateTime.Today);

    /// <summary>
    /// Gets the expected end date based on start date and term length.
    /// </summary>
    [NotMapped]
    public DateTime ExpectedEndDate => AssignmentStartDate.AddYears(TermLengthYears);

    /// <summary>
    /// Gets the number of days remaining in the current assignment.
    /// </summary>
    [NotMapped]
    public int DaysRemainingInTerm => AssignmentEndDate.HasValue
        ? Math.Max(0, (AssignmentEndDate.Value - DateTime.Today).Days)
        : Math.Max(0, (ExpectedEndDate - DateTime.Today).Days);

    /// <summary>
    /// Gets whether the assignment is nearing expiration.
    /// </summary>
    [NotMapped]
    public bool IsNearingExpiration => DaysRemainingInTerm <= 365 && DaysRemainingInTerm > 0;

    /// <summary>
    /// Gets whether this is an interim or acting appointment.
    /// </summary>
    [NotMapped]
    public bool IsInterimAppointment => AppointmentType == "Interim" || AppointmentType == "Acting";

    /// <summary>
    /// Gets the total compensation including salary and stipend.
    /// </summary>
    [NotMapped]
    public decimal? TotalCompensation => (ActualSalary ?? 0) + (AdministrativeStipend ?? 0);

    /// <summary>
    /// Gets the total years in this administrative role.
    /// </summary>
    [NotMapped]
    public int TotalYearsInRole => (int)Math.Ceiling(((AssignmentEndDate ?? DateTime.Today) - AssignmentStartDate).TotalDays / 365.0);

    /// <summary>
    /// Gets whether performance review is due within 30 days.
    /// </summary>
    [NotMapped]
    public bool IsPerformanceReviewDue => NextPerformanceReview.HasValue
        && NextPerformanceReview.Value <= DateTime.Today.AddDays(30);
}