using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a vote cast by a committee member on a promotion application.
/// Manages voting records, recommendations, and decision tracking.
/// </summary>
public class PromotionVote : BaseEntity
{
    /// <summary>
    /// Gets or sets the promotion application ID.
    /// </summary>
    [Required]
    public int PromotionApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the promotion committee member ID.
    /// </summary>
    [Required]
    public int PromotionCommitteeMemberId { get; set; }

    /// <summary>
    /// Gets or sets the employee number of the voter.
    /// </summary>
    [Required]
    public int VoterEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the voting session ID.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string VotingSessionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vote cast.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Vote { get; set; } = string.Empty; // Approve, Reject, Abstain, Recuse

    /// <summary>
    /// Gets or sets the vote date and time.
    /// </summary>
    [Required]
    public DateTime VoteDateTime { get; set; }

    /// <summary>
    /// Gets or sets the voting method used.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string VotingMethod { get; set; } = "Meeting"; // Meeting, Electronic, Written

    /// <summary>
    /// Gets or sets whether the vote is confidential.
    /// </summary>
    [Required]
    public bool IsConfidential { get; set; } = true;

    /// <summary>
    /// Gets or sets the vote weight (usually 1.0 for equal votes).
    /// </summary>
    [Column(TypeName = "decimal(3,2)")]
    public decimal VoteWeight { get; set; } = 1.0m;

    /// <summary>
    /// Gets or sets the confidence level in the vote.
    /// </summary>
    [StringLength(20)]
    public string? ConfidenceLevel { get; set; } // High, Medium, Low

    /// <summary>
    /// Gets or sets the primary reason for the vote.
    /// </summary>
    [StringLength(100)]
    public string? PrimaryReason { get; set; }

    /// <summary>
    /// Gets or sets detailed comments supporting the vote.
    /// </summary>
    [StringLength(2000)]
    public string? Comments { get; set; }

    /// <summary>
    /// Gets or sets specific strengths identified.
    /// </summary>
    [StringLength(1000)]
    public string? IdentifiedStrengths { get; set; }

    /// <summary>
    /// Gets or sets specific concerns or weaknesses identified.
    /// </summary>
    [StringLength(1000)]
    public string? IdentifiedConcerns { get; set; }

    /// <summary>
    /// Gets or sets recommendations for the candidate.
    /// </summary>
    [StringLength(1000)]
    public string? Recommendations { get; set; }

    /// <summary>
    /// Gets or sets the evaluation of teaching effectiveness.
    /// </summary>
    [StringLength(20)]
    public string? TeachingEvaluation { get; set; } // Excellent, Good, Satisfactory, Needs Improvement

    /// <summary>
    /// Gets or sets the evaluation of research/scholarship.
    /// </summary>
    [StringLength(20)]
    public string? ResearchEvaluation { get; set; }

    /// <summary>
    /// Gets or sets the evaluation of service contribution.
    /// </summary>
    [StringLength(20)]
    public string? ServiceEvaluation { get; set; }

    /// <summary>
    /// Gets or sets the overall evaluation score.
    /// </summary>
    [Column(TypeName = "decimal(4,2)")]
    public decimal? OverallScore { get; set; } // Scale 0-10 or 0-100

    /// <summary>
    /// Gets or sets whether the voter has a conflict of interest.
    /// </summary>
    public bool HasConflictOfInterest { get; set; } = false;

    /// <summary>
    /// Gets or sets the conflict of interest description.
    /// </summary>
    [StringLength(500)]
    public string? ConflictOfInterestDescription { get; set; }

    /// <summary>
    /// Gets or sets whether the voter recused themselves.
    /// </summary>
    public bool IsRecused { get; set; } = false;

    /// <summary>
    /// Gets or sets the recusal reason.
    /// </summary>
    [StringLength(500)]
    public string? RecusalReason { get; set; }

    /// <summary>
    /// Gets or sets whether external consultation was sought.
    /// </summary>
    public bool ExternalConsultationSought { get; set; } = false;

    /// <summary>
    /// Gets or sets external consultation details.
    /// </summary>
    [StringLength(500)]
    public string? ExternalConsultationDetails { get; set; }

    /// <summary>
    /// Gets or sets the areas requiring additional evidence.
    /// </summary>
    [StringLength(1000)]
    public string? AdditionalEvidenceNeeded { get; set; }

    /// <summary>
    /// Gets or sets suggested improvements for the application.
    /// </summary>
    [StringLength(1000)]
    public string? SuggestedImprovements { get; set; }

    /// <summary>
    /// Gets or sets the time spent reviewing the application (in hours).
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? ReviewTimeHours { get; set; }

    /// <summary>
    /// Gets or sets the documents reviewed.
    /// </summary>
    [StringLength(500)]
    public string? DocumentsReviewed { get; set; }

    /// <summary>
    /// Gets or sets whether the vote can be changed.
    /// </summary>
    public bool CanChangeVote { get; set; } = false;

    /// <summary>
    /// Gets or sets when the vote was last modified.
    /// </summary>
    public DateTime? LastModifiedDateTime { get; set; }

    /// <summary>
    /// Gets or sets the number of times the vote was changed.
    /// </summary>
    public int VoteChangeCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the vote submission method.
    /// </summary>
    [StringLength(50)]
    public string? SubmissionMethod { get; set; } // System, Email, Paper

    /// <summary>
    /// Gets or sets the IP address of the voter (for electronic votes).
    /// </summary>
    [StringLength(45)]
    public string? VoterIPAddress { get; set; }

    /// <summary>
    /// Gets or sets the vote verification code.
    /// </summary>
    [StringLength(100)]
    public string? VerificationCode { get; set; }

    /// <summary>
    /// Gets or sets whether the vote has been verified.
    /// </summary>
    public bool IsVerified { get; set; } = false;

    /// <summary>
    /// Gets or sets additional metadata about the vote.
    /// </summary>
    [StringLength(1000)]
    public string? VoteMetadata { get; set; }

    /// <summary>
    /// Gets or sets notes about this vote.
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the promotion application.
    /// </summary>
    [ForeignKey(nameof(PromotionApplicationId))]
    public PromotionApplication PromotionApplication { get; set; } = null!;

    /// <summary>
    /// Gets or sets the promotion committee member.
    /// </summary>
    [ForeignKey(nameof(PromotionCommitteeMemberId))]
    public PromotionCommitteeMember PromotionCommitteeMember { get; set; } = null!;

    /// <summary>
    /// Gets or sets the voter.
    /// </summary>
    [ForeignKey(nameof(VoterEmpNr))]
    public Academic Voter { get; set; } = null!;

    // Computed properties
    /// <summary>
    /// Gets whether this is a positive vote.
    /// </summary>
    [NotMapped]
    public bool IsPositiveVote => Vote == "Approve" || Vote == "Support";

    /// <summary>
    /// Gets whether this is a negative vote.
    /// </summary>
    [NotMapped]
    public bool IsNegativeVote => Vote == "Reject" || Vote == "Deny";

    /// <summary>
    /// Gets whether the voter abstained.
    /// </summary>
    [NotMapped]
    public bool IsAbstention => Vote == "Abstain";

    /// <summary>
    /// Gets whether the vote counts toward the total.
    /// </summary>
    [NotMapped]
    public bool CountsTowardTotal => !IsRecused && Vote != "Abstain";

    /// <summary>
    /// Gets the vote's effective weight.
    /// </summary>
    [NotMapped]
    public decimal EffectiveWeight => CountsTowardTotal ? VoteWeight : 0m;

    /// <summary>
    /// Gets whether the vote is final.
    /// </summary>
    [NotMapped]
    public bool IsFinal => !CanChangeVote || IsVerified;

    /// <summary>
    /// Gets the vote age in days.
    /// </summary>
    [NotMapped]
    public int VoteAgeDays => (int)(DateTime.Now - VoteDateTime).TotalDays;

    /// <summary>
    /// Gets whether the vote is recent (within 30 days).
    /// </summary>
    [NotMapped]
    public bool IsRecentVote => VoteAgeDays <= 30;

    /// <summary>
    /// Gets the vote summary.
    /// </summary>
    [NotMapped]
    public string VoteSummary
    {
        get
        {
            var summary = Vote;
            if (HasConflictOfInterest) summary += " (COI)";
            if (IsRecused) summary += " (Recused)";
            if (!string.IsNullOrEmpty(ConfidenceLevel)) summary += $" ({ConfidenceLevel} confidence)";
            return summary;
        }
    }

    /// <summary>
    /// Gets the vote strength based on confidence and score.
    /// </summary>
    [NotMapped]
    public string VoteStrength
    {
        get
        {
            if (IsRecused || Vote == "Abstain") return "N/A";

            var strength = ConfidenceLevel switch
            {
                "High" => "Strong",
                "Medium" => "Moderate",
                "Low" => "Weak",
                _ => "Unspecified"
            };

            return $"{strength} {Vote}";
        }
    }

    /// <summary>
    /// Gets whether the vote has detailed feedback.
    /// </summary>
    [NotMapped]
    public bool HasDetailedFeedback => !string.IsNullOrEmpty(Comments) ||
                                      !string.IsNullOrEmpty(IdentifiedStrengths) ||
                                      !string.IsNullOrEmpty(IdentifiedConcerns);

    /// <summary>
    /// Gets the completeness score of the vote (0-100).
    /// </summary>
    [NotMapped]
    public int CompletenessScore
    {
        get
        {
            int score = 30; // Base score for casting a vote

            if (!string.IsNullOrEmpty(PrimaryReason)) score += 15;
            if (!string.IsNullOrEmpty(Comments)) score += 15;
            if (!string.IsNullOrEmpty(IdentifiedStrengths)) score += 10;
            if (!string.IsNullOrEmpty(IdentifiedConcerns)) score += 10;
            if (!string.IsNullOrEmpty(Recommendations)) score += 10;
            if (OverallScore.HasValue) score += 10;

            return Math.Min(score, 100);
        }
    }

    /// <summary>
    /// Gets the vote quality rating.
    /// </summary>
    [NotMapped]
    public string QualityRating
    {
        get
        {
            var score = CompletenessScore;
            return score switch
            {
                >= 90 => "Comprehensive",
                >= 75 => "Detailed",
                >= 60 => "Adequate",
                >= 45 => "Basic",
                _ => "Minimal"
            };
        }
    }
}