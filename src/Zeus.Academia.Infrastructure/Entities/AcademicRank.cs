using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing academic rank assignments and progression for faculty members.
/// Manages rank progression from Assistant Professor to Full Professor with tenure status tracking.
/// </summary>
public class AcademicRank : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic employee number this rank assignment belongs to.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the academic rank level.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string RankLevel { get; set; } = string.Empty; // Assistant Professor, Associate Professor, Full Professor

    /// <summary>
    /// Gets or sets the rank category or series.
    /// </summary>
    [StringLength(50)]
    public string? RankCategory { get; set; } // Teaching, Research, Clinical, Adjunct

    /// <summary>
    /// Gets or sets the tenure status.
    /// </summary>
    [Required]
    [StringLength(30)]
    public string TenureStatus { get; set; } = "Non-Tenure Track"; // Non-Tenure Track, Tenure Track, Tenured

    /// <summary>
    /// Gets or sets the date when this rank was effective from.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets the date when this rank assignment ends (null for current rank).
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this is the current active rank.
    /// </summary>
    [Required]
    public bool IsCurrentRank { get; set; } = true;

    /// <summary>
    /// Gets or sets the reason for the rank assignment.
    /// </summary>
    [StringLength(100)]
    public string? AssignmentReason { get; set; } // Initial Hire, Promotion, Lateral Transfer, etc.

    /// <summary>
    /// Gets or sets the promotion committee ID that approved this rank (if applicable).
    /// </summary>
    public int? PromotionCommitteeId { get; set; }

    /// <summary>
    /// Gets or sets the expected promotion review date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? NextReviewDate { get; set; }

    /// <summary>
    /// Gets or sets the minimum years in rank before promotion eligibility.
    /// </summary>
    public int? MinimumYearsInRank { get; set; }

    /// <summary>
    /// Gets or sets the annual salary associated with this rank.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? AnnualSalary { get; set; }

    /// <summary>
    /// Gets or sets the salary grade or step within the rank.
    /// </summary>
    [StringLength(20)]
    public string? SalaryGrade { get; set; }

    /// <summary>
    /// Gets or sets the department code for this rank assignment.
    /// </summary>
    [StringLength(10)]
    public string? DepartmentCode { get; set; }

    /// <summary>
    /// Gets or sets the college or school code.
    /// </summary>
    [StringLength(10)]
    public string? CollegeCode { get; set; }

    /// <summary>
    /// Gets or sets the appointment percentage (100 for full-time, less for part-time).
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal AppointmentPercentage { get; set; } = 100.00m;

    /// <summary>
    /// Gets or sets whether this rank is eligible for sabbatical leave.
    /// </summary>
    [Required]
    public bool IsSabbaticalEligible { get; set; } = false;

    /// <summary>
    /// Gets or sets the voting rights level for faculty governance.
    /// </summary>
    [StringLength(30)]
    public string? VotingRights { get; set; } // Full Voting, Limited Voting, No Vote

    /// <summary>
    /// Gets or sets additional rank qualifications or certifications.
    /// </summary>
    [StringLength(500)]
    public string? RankQualifications { get; set; }

    /// <summary>
    /// Gets or sets notes about this rank assignment.
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the academic faculty member this rank belongs to.
    /// </summary>
    [ForeignKey(nameof(AcademicEmpNr))]
    public Academic Academic { get; set; } = null!;

    /// <summary>
    /// Gets or sets the promotion committee that approved this rank (if applicable).
    /// </summary>
    public PromotionCommittee? PromotionCommittee { get; set; }

    // Computed properties
    /// <summary>
    /// Gets the number of years in the current rank.
    /// </summary>
    [NotMapped]
    public int YearsInRank => EndDate.HasValue
        ? (EndDate.Value - EffectiveDate).Days / 365
        : (DateTime.Today - EffectiveDate).Days / 365;

    /// <summary>
    /// Gets whether the faculty member is eligible for promotion review.
    /// </summary>
    [NotMapped]
    public bool IsEligibleForPromotion => IsCurrentRank &&
                                         MinimumYearsInRank.HasValue &&
                                         YearsInRank >= MinimumYearsInRank.Value;

    /// <summary>
    /// Gets whether the faculty member has tenure.
    /// </summary>
    [NotMapped]
    public bool HasTenure => TenureStatus == "Tenured";

    /// <summary>
    /// Gets whether the faculty member is on the tenure track.
    /// </summary>
    [NotMapped]
    public bool IsOnTenureTrack => TenureStatus == "Tenure Track";

    /// <summary>
    /// Gets the full rank description.
    /// </summary>
    [NotMapped]
    public string FullRankDescription => !string.IsNullOrEmpty(RankCategory)
        ? $"{RankLevel} ({RankCategory})"
        : RankLevel;

    /// <summary>
    /// Gets whether this is a senior faculty rank (Associate or Full Professor).
    /// </summary>
    [NotMapped]
    public bool IsSeniorRank => RankLevel.Contains("Associate Professor") ||
                               RankLevel.Contains("Full Professor") ||
                               RankLevel.Contains("Professor");

    /// <summary>
    /// Gets the appointment status description.
    /// </summary>
    [NotMapped]
    public string AppointmentStatus => AppointmentPercentage >= 100m
        ? "Full-time"
        : $"Part-time ({AppointmentPercentage}%)";
}