using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing faculty promotion history and tracking.
/// Manages promotion workflows, rank changes, and tenure status changes.
/// </summary>
public class FacultyPromotion : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic employee number this promotion record belongs to.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the promotion type (Rank, Tenure, Administrative, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string PromotionType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the previous rank code.
    /// </summary>
    [MaxLength(10)]
    public string? FromRankCode { get; set; }

    /// <summary>
    /// Gets or sets the new rank code after promotion.
    /// </summary>
    [MaxLength(10)]
    public string? ToRankCode { get; set; }

    /// <summary>
    /// Gets or sets the previous tenure status.
    /// </summary>
    public bool? FromTenureStatus { get; set; }

    /// <summary>
    /// Gets or sets the new tenure status after promotion.
    /// </summary>
    public bool? ToTenureStatus { get; set; }

    /// <summary>
    /// Gets or sets the effective date of the promotion.
    /// </summary>
    [Required]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets the date the promotion application was submitted.
    /// </summary>
    public DateTime? ApplicationDate { get; set; }

    /// <summary>
    /// Gets or sets the current status of the promotion (Pending, Under Review, Approved, Denied, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets the department recommendation (Approved, Not Approved, etc.).
    /// </summary>
    [MaxLength(50)]
    public string? DepartmentRecommendation { get; set; }

    /// <summary>
    /// Gets or sets the college recommendation.
    /// </summary>
    [MaxLength(50)]
    public string? CollegeRecommendation { get; set; }

    /// <summary>
    /// Gets or sets the university-level recommendation.
    /// </summary>
    [MaxLength(50)]
    public string? UniversityRecommendation { get; set; }

    /// <summary>
    /// Gets or sets the final decision.
    /// </summary>
    [MaxLength(50)]
    public string? FinalDecision { get; set; }

    /// <summary>
    /// Gets or sets who made the final decision.
    /// </summary>
    [MaxLength(100)]
    public string? DecisionMadeBy { get; set; }

    /// <summary>
    /// Gets or sets the date of the final decision.
    /// </summary>
    public DateTime? DecisionDate { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the promotion.
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the justification or criteria met for the promotion.
    /// </summary>
    [MaxLength(2000)]
    public string? Justification { get; set; }

    /// <summary>
    /// Gets or sets supporting documents or references.
    /// </summary>
    [MaxLength(1000)]
    public string? SupportingDocuments { get; set; }

    /// <summary>
    /// Navigation property to the academic employee.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;

    /// <summary>
    /// Navigation property to the previous rank.
    /// </summary>
    public virtual Rank? FromRank { get; set; }

    /// <summary>
    /// Navigation property to the new rank.
    /// </summary>
    public virtual Rank? ToRank { get; set; }
}