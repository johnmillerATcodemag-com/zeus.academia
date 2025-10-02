using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic term/semester for enrollment management
/// </summary>
public class AcademicTerm : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the term.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the term code (e.g., "FALL2025", "SPRING2026").
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string TermCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable term name (e.g., "Fall 2025").
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TermName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of term (Fall, Spring, Summer, etc.).
    /// </summary>
    [Required]
    public TermType TermType { get; set; }

    /// <summary>
    /// Gets or sets the academic year.
    /// </summary>
    [Required]
    public int AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the start date of the term.
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the term.
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the application deadline for this term.
    /// </summary>
    public DateTime? ApplicationDeadline { get; set; }

    /// <summary>
    /// Gets or sets the early application deadline.
    /// </summary>
    public DateTime? EarlyApplicationDeadline { get; set; }

    /// <summary>
    /// Gets or sets the enrollment start date (when students can begin enrolling).
    /// </summary>
    public DateTime? EnrollmentStartDate { get; set; }

    /// <summary>
    /// Gets or sets the enrollment deadline.
    /// </summary>
    public DateTime? EnrollmentDeadline { get; set; }

    /// <summary>
    /// Gets or sets the late enrollment deadline (with penalty).
    /// </summary>
    public DateTime? LateEnrollmentDeadline { get; set; }

    /// <summary>
    /// Gets or sets the last date to drop courses without penalty.
    /// </summary>
    public DateTime? DropDeadline { get; set; }

    /// <summary>
    /// Gets or sets the last date to withdraw from courses.
    /// </summary>
    public DateTime? WithdrawDeadline { get; set; }

    /// <summary>
    /// Gets or sets whether this term is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this is the current term.
    /// </summary>
    public bool IsCurrent { get; set; } = false;

    /// <summary>
    /// Gets or sets whether applications are open for this term.
    /// </summary>
    public bool ApplicationsOpen { get; set; } = false;

    /// <summary>
    /// Gets or sets whether enrollment is open for this term.
    /// </summary>
    public bool EnrollmentOpen { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum number of credits a student can take this term.
    /// </summary>
    public int? MaxCreditsPerStudent { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of credits for full-time status.
    /// </summary>
    public int? MinCreditsFullTime { get; set; } = 12;

    /// <summary>
    /// Gets or sets additional term-specific notes or announcements.
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the tuition and fees for this term.
    /// </summary>
    public decimal? TuitionAmount { get; set; }

    /// <summary>
    /// Gets or sets the late enrollment fee.
    /// </summary>
    public decimal? LateEnrollmentFee { get; set; }

    /// <summary>
    /// Navigation property to enrollment applications for this term.
    /// </summary>
    public virtual ICollection<EnrollmentApplication> Applications { get; set; } = new List<EnrollmentApplication>();

    /// <summary>
    /// Navigation property to student enrollments for this term.
    /// </summary>
    public virtual ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();

    /// <summary>
    /// Navigation property to enrollment history for this term.
    /// </summary>
    public virtual ICollection<EnrollmentHistory> EnrollmentHistory { get; set; } = new List<EnrollmentHistory>();
}