using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing department chair assignments and rotation system.
/// Manages the appointment and tenure of department chairs, separate from endowed Chair positions.
/// </summary>
public class DepartmentChair : BaseEntity
{
    /// <summary>
    /// Gets or sets the department name that this chair leads.
    /// </summary>
    [Required]
    [StringLength(15)]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee number of the faculty member serving as chair.
    /// </summary>
    [Required]
    public int FacultyEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the start date of the chair appointment.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime AppointmentStartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the chair appointment.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? AppointmentEndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this is the current active chair assignment.
    /// </summary>
    [Required]
    public bool IsCurrent { get; set; } = true;

    /// <summary>
    /// Gets or sets the term length in years (typically 3-5 years).
    /// </summary>
    [Range(1, 10)]
    public int TermLengthYears { get; set; } = 3;

    /// <summary>
    /// Gets or sets whether the chair is eligible for reappointment.
    /// </summary>
    public bool IsEligibleForRenewal { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of times this person has served as chair.
    /// </summary>
    [Range(1, 20)]
    public int TermNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the appointment type (Initial, Reappointment, Interim, Acting).
    /// </summary>
    [Required]
    [StringLength(20)]
    public string AppointmentType { get; set; } = "Initial"; // Initial, Reappointment, Interim, Acting

    /// <summary>
    /// Gets or sets who appointed this chair (Provost, Dean, Faculty Vote, etc.).
    /// </summary>
    [StringLength(100)]
    public string? AppointedBy { get; set; }

    /// <summary>
    /// Gets or sets the appointment method (Administrative, Faculty Election, Search Committee).
    /// </summary>
    [StringLength(50)]
    public string? AppointmentMethod { get; set; }

    /// <summary>
    /// Gets or sets the reason for appointment change (Term Completion, Resignation, Promotion, etc.).
    /// </summary>
    [StringLength(100)]
    public string? ChangeReason { get; set; }

    /// <summary>
    /// Gets or sets the date when the chair appointment was announced.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? AnnouncementDate { get; set; }

    /// <summary>
    /// Gets or sets the salary adjustment or stipend for the chair role.
    /// </summary>
    [Column(TypeName = "decimal(12,2)")]
    public decimal? ChairStipend { get; set; }

    /// <summary>
    /// Gets or sets whether the chair receives course release time.
    /// </summary>
    public bool ReceivesCourseRelease { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of courses released per term.
    /// </summary>
    [Range(0, 6)]
    public int CourseReleaseCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets additional benefits or support provided to the chair.
    /// </summary>
    [StringLength(500)]
    public string? AdditionalBenefits { get; set; }

    /// <summary>
    /// Gets or sets special responsibilities or initiatives for this chair term.
    /// </summary>
    [StringLength(1000)]
    public string? SpecialResponsibilities { get; set; }

    /// <summary>
    /// Gets or sets transition notes for incoming/outgoing chairs.
    /// </summary>
    [StringLength(2000)]
    public string? TransitionNotes { get; set; }

    /// <summary>
    /// Gets or sets performance evaluation notes for the chair's service.
    /// </summary>
    [StringLength(2000)]
    public string? PerformanceNotes { get; set; }

    /// <summary>
    /// Gets or sets the date of the last performance review.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? LastReviewDate { get; set; }

    /// <summary>
    /// Gets or sets the next scheduled review date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? NextReviewDate { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department Department { get; set; } = null!;

    /// <summary>
    /// Navigation property to the faculty member serving as chair.
    /// </summary>
    public virtual Academic Faculty { get; set; } = null!;

    // Computed properties
    /// <summary>
    /// Gets whether the chair appointment is currently active.
    /// </summary>
    [NotMapped]
    public bool IsActive => IsCurrent && (AppointmentEndDate == null || AppointmentEndDate > DateTime.Today);

    /// <summary>
    /// Gets the expected end date based on start date and term length.
    /// </summary>
    [NotMapped]
    public DateTime ExpectedEndDate => AppointmentStartDate.AddYears(TermLengthYears);

    /// <summary>
    /// Gets the number of days remaining in the current term.
    /// </summary>
    [NotMapped]
    public int DaysRemainingInTerm => AppointmentEndDate.HasValue
        ? Math.Max(0, (AppointmentEndDate.Value - DateTime.Today).Days)
        : Math.Max(0, (ExpectedEndDate - DateTime.Today).Days);

    /// <summary>
    /// Gets whether the chair term is nearing expiration (within 6 months).
    /// </summary>
    [NotMapped]
    public bool IsNearingExpiration => DaysRemainingInTerm <= 180 && DaysRemainingInTerm > 0;

    /// <summary>
    /// Gets whether this chair appointment is interim or acting.
    /// </summary>
    [NotMapped]
    public bool IsInterimAppointment => AppointmentType == "Interim" || AppointmentType == "Acting";

    /// <summary>
    /// Gets the total years of service as chair across all terms.
    /// </summary>
    [NotMapped]
    public int TotalYearsAsChair => (int)Math.Ceiling(((AppointmentEndDate ?? DateTime.Today) - AppointmentStartDate).TotalDays / 365.0);
}