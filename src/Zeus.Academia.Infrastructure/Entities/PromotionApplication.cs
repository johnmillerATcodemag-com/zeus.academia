using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a faculty promotion application with workflow stages and approval tracking.
/// Manages the promotion process from application submission through final approval.
/// </summary>
public class PromotionApplication : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic employee number of the faculty member applying for promotion.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the current rank level of the applicant.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string CurrentRank { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the requested rank level for promotion.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string RequestedRank { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of promotion application.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ApplicationType { get; set; } = string.Empty; // Regular Promotion, Early Promotion, Tenure and Promotion

    /// <summary>
    /// Gets or sets the academic year for which promotion is sought.
    /// </summary>
    [Required]
    [StringLength(9)]
    public string AcademicYear { get; set; } = string.Empty; // e.g., "2024-2025"

    /// <summary>
    /// Gets or sets the date the application was submitted.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime ApplicationDate { get; set; }

    /// <summary>
    /// Gets or sets the application deadline.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime ApplicationDeadline { get; set; }

    /// <summary>
    /// Gets or sets the current status of the application.
    /// </summary>
    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Under Review, Approved, Denied, Withdrawn

    /// <summary>
    /// Gets or sets the current workflow stage.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string WorkflowStage { get; set; } = "Application Preparation"; // Application Preparation, Department Review, College Review, University Review, Final Decision

    /// <summary>
    /// Gets or sets the promotion committee ID assigned to review this application.
    /// </summary>
    public int? PromotionCommitteeId { get; set; }

    /// <summary>
    /// Gets or sets the expected decision date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? ExpectedDecisionDate { get; set; }

    /// <summary>
    /// Gets or sets the actual decision date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? DecisionDate { get; set; }

    /// <summary>
    /// Gets or sets the final decision on the application.
    /// </summary>
    [StringLength(30)]
    public string? FinalDecision { get; set; } // Approved, Denied, Deferred

    /// <summary>
    /// Gets or sets the decision rationale or comments.
    /// </summary>
    [StringLength(2000)]
    public string? DecisionRationale { get; set; }

    /// <summary>
    /// Gets or sets the candidate's self-statement or cover letter.
    /// </summary>
    [StringLength(5000)]
    public string? CandidateStatement { get; set; }

    /// <summary>
    /// Gets or sets the teaching portfolio summary.
    /// </summary>
    [StringLength(2000)]
    public string? TeachingPortfolio { get; set; }

    /// <summary>
    /// Gets or sets the research portfolio summary.
    /// </summary>
    [StringLength(2000)]
    public string? ResearchPortfolio { get; set; }

    /// <summary>
    /// Gets or sets the service portfolio summary.
    /// </summary>
    [StringLength(2000)]
    public string? ServicePortfolio { get; set; }

    /// <summary>
    /// Gets or sets the list of external referees.
    /// </summary>
    [StringLength(1000)]
    public string? ExternalReferees { get; set; }

    /// <summary>
    /// Gets or sets the number of years in current rank.
    /// </summary>
    public int YearsInCurrentRank { get; set; }

    /// <summary>
    /// Gets or sets the number of publications since last promotion.
    /// </summary>
    public int PublicationsCount { get; set; }

    /// <summary>
    /// Gets or sets the teaching evaluation average.
    /// </summary>
    [Column(TypeName = "decimal(4,2)")]
    public decimal? TeachingEvaluationAverage { get; set; }

    /// <summary>
    /// Gets or sets the grant funding amount obtained.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? GrantFundingAmount { get; set; }

    /// <summary>
    /// Gets or sets whether tenure is also being sought.
    /// </summary>
    [Required]
    public bool IsSeekingTenure { get; set; } = false;

    /// <summary>
    /// Gets or sets the mentor assigned to guide the candidate.
    /// </summary>
    public int? MentorEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the department recommendation.
    /// </summary>
    [StringLength(30)]
    public string? DepartmentRecommendation { get; set; } // Strongly Support, Support, Do Not Support

    /// <summary>
    /// Gets or sets the college recommendation.
    /// </summary>
    [StringLength(30)]
    public string? CollegeRecommendation { get; set; }

    /// <summary>
    /// Gets or sets the university-level recommendation.
    /// </summary>
    [StringLength(30)]
    public string? UniversityRecommendation { get; set; }

    /// <summary>
    /// Gets or sets the notification preferences for the candidate.
    /// </summary>
    [StringLength(500)]
    public string? NotificationPreferences { get; set; }

    /// <summary>
    /// Gets or sets additional documents or notes.
    /// </summary>
    [StringLength(1000)]
    public string? AdditionalNotes { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the academic faculty member applying for promotion.
    /// </summary>
    [ForeignKey(nameof(AcademicEmpNr))]
    public Academic Academic { get; set; } = null!;

    /// <summary>
    /// Gets or sets the promotion committee reviewing this application.
    /// </summary>
    public PromotionCommittee? PromotionCommittee { get; set; }

    /// <summary>
    /// Gets or sets the mentor faculty member (if assigned).
    /// </summary>
    [ForeignKey(nameof(MentorEmpNr))]
    public Academic? Mentor { get; set; }

    /// <summary>
    /// Gets or sets the collection of workflow steps for this application.
    /// </summary>
    public ICollection<PromotionWorkflowStep> WorkflowSteps { get; set; } = new List<PromotionWorkflowStep>();

    /// <summary>
    /// Gets or sets the collection of votes for this application.
    /// </summary>
    public ICollection<PromotionVote> Votes { get; set; } = new List<PromotionVote>();

    // Computed properties
    /// <summary>
    /// Gets the number of days since the application was submitted.
    /// </summary>
    [NotMapped]
    public int DaysSinceSubmission => Status != "Draft"
        ? (DateTime.Today - ApplicationDate).Days
        : 0;

    /// <summary>
    /// Gets whether the application is overdue for a decision.
    /// </summary>
    [NotMapped]
    public bool IsOverdue => ExpectedDecisionDate.HasValue &&
                            DateTime.Today > ExpectedDecisionDate.Value &&
                            string.IsNullOrEmpty(FinalDecision);

    /// <summary>
    /// Gets whether the application is still active (not decided, withdrawn, or expired).
    /// </summary>
    [NotMapped]
    public bool IsActive => Status != "Withdrawn" &&
                           string.IsNullOrEmpty(FinalDecision) &&
                           Status != "Draft";

    /// <summary>
    /// Gets the application progress percentage based on workflow stage.
    /// </summary>
    [NotMapped]
    public int ProgressPercentage => WorkflowStage switch
    {
        "Application Preparation" => 10,
        "Submitted" => 20,
        "Department Review" => 40,
        "College Review" => 60,
        "University Review" => 80,
        "Final Decision" => 100,
        _ => 0
    };

    /// <summary>
    /// Gets whether all required portfolios are provided.
    /// </summary>
    [NotMapped]
    public bool HasCompletePortfolio => !string.IsNullOrEmpty(TeachingPortfolio) &&
                                       !string.IsNullOrEmpty(ResearchPortfolio) &&
                                       !string.IsNullOrEmpty(ServicePortfolio);

    /// <summary>
    /// Gets the expected rank progression description.
    /// </summary>
    [NotMapped]
    public string PromotionDescription => $"{CurrentRank} → {RequestedRank}";
}