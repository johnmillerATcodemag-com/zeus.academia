using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing tenure track status and timeline management for faculty members.
/// Manages tenure track milestones, evaluation periods, and tenure decision tracking.
/// </summary>
public class TenureTrack : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic employee number of the faculty member on tenure track.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the tenure track start date (usually hire date for tenure-track position).
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime TenureTrackStartDate { get; set; }

    /// <summary>
    /// Gets or sets the expected tenure decision date (typically 6 years from start).
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime ExpectedTenureDate { get; set; }

    /// <summary>
    /// Gets or sets the actual tenure decision date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? ActualTenureDate { get; set; }

    /// <summary>
    /// Gets or sets the current tenure status.
    /// </summary>
    [Required]
    [StringLength(30)]
    public string TenureStatus { get; set; } = "On Track"; // On Track, Under Review, Approved, Denied, Extended, Terminated

    /// <summary>
    /// Gets or sets the tenure clock status.
    /// </summary>
    [Required]
    [StringLength(30)]
    public string ClockStatus { get; set; } = "Running"; // Running, Stopped, Extended, Reset

    /// <summary>
    /// Gets or sets the number of years on tenure track.
    /// </summary>
    [Column(TypeName = "decimal(4,2)")]
    public decimal YearsOnTrack { get; set; }

    /// <summary>
    /// Gets or sets the maximum years allowed on tenure track.
    /// </summary>
    [Column(TypeName = "decimal(4,2)")]
    public decimal MaxYearsAllowed { get; set; } = 6.0m;

    /// <summary>
    /// Gets or sets the tenure application ID if submitted.
    /// </summary>
    public int? TenureApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the first-year review date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? FirstYearReviewDate { get; set; }

    /// <summary>
    /// Gets or sets the first-year review outcome.
    /// </summary>
    [StringLength(30)]
    public string? FirstYearReviewOutcome { get; set; } // Satisfactory, Needs Improvement, Unsatisfactory

    /// <summary>
    /// Gets or sets the third-year review date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? ThirdYearReviewDate { get; set; }

    /// <summary>
    /// Gets or sets the third-year review outcome.
    /// </summary>
    [StringLength(30)]
    public string? ThirdYearReviewOutcome { get; set; }

    /// <summary>
    /// Gets or sets the sixth-year (tenure) review date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? SixthYearReviewDate { get; set; }

    /// <summary>
    /// Gets or sets the sixth-year review outcome.
    /// </summary>
    [StringLength(30)]
    public string? SixthYearReviewOutcome { get; set; }

    /// <summary>
    /// Gets or sets the assigned tenure mentor.
    /// </summary>
    public int? MentorEmpNr { get; set; }

    /// <summary>
    /// Gets or sets any tenure clock extensions granted.
    /// </summary>
    [Column(TypeName = "decimal(4,2)")]
    public decimal? ClockExtensionYears { get; set; }

    /// <summary>
    /// Gets or sets the reason for any clock extension.
    /// </summary>
    [StringLength(500)]
    public string? ExtensionReason { get; set; } // Parental Leave, Medical Leave, Research Delay, etc.

    /// <summary>
    /// Gets or sets the probationary period end date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? ProbationaryEndDate { get; set; }

    /// <summary>
    /// Gets or sets whether the faculty member is eligible for early tenure consideration.
    /// </summary>
    [Required]
    public bool IsEligibleForEarlyTenure { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this tenure track record is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the department's tenure recommendation.
    /// </summary>
    [StringLength(30)]
    public string? DepartmentTenureRecommendation { get; set; }

    /// <summary>
    /// Gets or sets the college's tenure recommendation.
    /// </summary>
    [StringLength(30)]
    public string? CollegeTenureRecommendation { get; set; }

    /// <summary>
    /// Gets or sets the university's tenure recommendation.
    /// </summary>
    [StringLength(30)]
    public string? UniversityTenureRecommendation { get; set; }

    /// <summary>
    /// Gets or sets the final tenure decision.
    /// </summary>
    [StringLength(30)]
    public string? FinalTenureDecision { get; set; } // Granted, Denied, Deferred

    /// <summary>
    /// Gets or sets the tenure decision rationale.
    /// </summary>
    [StringLength(2000)]
    public string? TenureDecisionRationale { get; set; }

    /// <summary>
    /// Gets or sets the notification date for tenure decision.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? TenureNotificationDate { get; set; }

    /// <summary>
    /// Gets or sets any special conditions or requirements.
    /// </summary>
    [StringLength(1000)]
    public string? SpecialConditions { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the tenure track.
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the academic faculty member on tenure track.
    /// </summary>
    [ForeignKey(nameof(AcademicEmpNr))]
    public Academic Academic { get; set; } = null!;

    /// <summary>
    /// Gets or sets the tenure mentor faculty member.
    /// </summary>
    [ForeignKey(nameof(MentorEmpNr))]
    public Academic? Mentor { get; set; }

    /// <summary>
    /// Gets or sets the tenure application if submitted.
    /// </summary>
    public PromotionApplication? TenureApplication { get; set; }

    /// <summary>
    /// Gets or sets the collection of milestone evaluations.
    /// </summary>
    public ICollection<TenureMilestone> Milestones { get; set; } = new List<TenureMilestone>();

    // Computed properties
    /// <summary>
    /// Gets the current tenure track progress as a percentage.
    /// </summary>
    [NotMapped]
    public decimal ProgressPercentage => Math.Min((YearsOnTrack / MaxYearsAllowed) * 100, 100);

    /// <summary>
    /// Gets the remaining years on tenure track.
    /// </summary>
    [NotMapped]
    public decimal RemainingYears => Math.Max(MaxYearsAllowed - YearsOnTrack, 0);

    /// <summary>
    /// Gets whether the tenure clock is currently running.
    /// </summary>
    [NotMapped]
    public bool IsClockRunning => ClockStatus == "Running";

    /// <summary>
    /// Gets whether the faculty member is approaching tenure review.
    /// </summary>
    [NotMapped]
    public bool IsApproachingTenureReview => RemainingYears <= 1.0m && TenureStatus == "On Track";

    /// <summary>
    /// Gets whether the tenure track period has expired.
    /// </summary>
    [NotMapped]
    public bool IsExpired => YearsOnTrack >= MaxYearsAllowed &&
                            string.IsNullOrEmpty(FinalTenureDecision);

    /// <summary>
    /// Gets whether tenure has been granted.
    /// </summary>
    [NotMapped]
    public bool HasTenure => FinalTenureDecision == "Granted";

    /// <summary>
    /// Gets the next required review type.
    /// </summary>
    [NotMapped]
    public string NextReviewType
    {
        get
        {
            var currentYear = YearsOnTrack;
            if (currentYear < 1.0m && !FirstYearReviewDate.HasValue)
                return "First Year Review";
            if (currentYear >= 2.5m && currentYear < 4.0m && !ThirdYearReviewDate.HasValue)
                return "Third Year Review";
            if (currentYear >= 5.5m && !SixthYearReviewDate.HasValue)
                return "Tenure Review";
            return "No Review Required";
        }
    }

    /// <summary>
    /// Gets whether the tenure track is in good standing.
    /// </summary>
    [NotMapped]
    public bool IsInGoodStanding => TenureStatus == "On Track" &&
                                   (FirstYearReviewOutcome != "Unsatisfactory" || !FirstYearReviewDate.HasValue) &&
                                   (ThirdYearReviewOutcome != "Unsatisfactory" || !ThirdYearReviewDate.HasValue);
}