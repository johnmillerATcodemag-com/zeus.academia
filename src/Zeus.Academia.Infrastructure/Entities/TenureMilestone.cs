using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a milestone in the tenure track process.
/// Manages key checkpoints and reviews in the tenure timeline.
/// </summary>
public class TenureMilestone : BaseEntity
{
    /// <summary>
    /// Gets or sets the tenure track ID.
    /// </summary>
    [Required]
    public int TenureTrackId { get; set; }

    /// <summary>
    /// Gets or sets the milestone sequence number.
    /// </summary>
    [Required]
    public int MilestoneOrder { get; set; }

    /// <summary>
    /// Gets or sets the name of the milestone.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string MilestoneName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the milestone.
    /// </summary>
    [StringLength(500)]
    public string? MilestoneDescription { get; set; }

    /// <summary>
    /// Gets or sets the type of milestone.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string MilestoneType { get; set; } = string.Empty; // Review, Evaluation, Notification, Decision

    /// <summary>
    /// Gets or sets the academic year when this milestone occurs.
    /// </summary>
    [Required]
    public int AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the specific year in the tenure track (1-6 typically).
    /// </summary>
    [Required]
    public int TenureYear { get; set; }

    /// <summary>
    /// Gets or sets the scheduled date for this milestone.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime ScheduledDate { get; set; }

    /// <summary>
    /// Gets or sets the actual completion date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? CompletionDate { get; set; }

    /// <summary>
    /// Gets or sets the milestone status.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Postponed, Cancelled

    /// <summary>
    /// Gets or sets whether this milestone is mandatory.
    /// </summary>
    [Required]
    public bool IsMandatory { get; set; } = true;

    /// <summary>
    /// Gets or sets the importance level of this milestone.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string ImportanceLevel { get; set; } = "Normal"; // Critical, High, Normal, Low

    /// <summary>
    /// Gets or sets the required preparation time in days.
    /// </summary>
    public int PreparationDays { get; set; } = 30;

    /// <summary>
    /// Gets or sets the expected duration in days.
    /// </summary>
    public int ExpectedDurationDays { get; set; } = 1;

    /// <summary>
    /// Gets or sets the responsible party for this milestone.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ResponsibleParty { get; set; } = string.Empty; // Faculty, Department, College, Committee

    /// <summary>
    /// Gets or sets the committee ID if applicable.
    /// </summary>
    public int? CommitteeId { get; set; }

    /// <summary>
    /// Gets or sets the reviewer employee number if applicable.
    /// </summary>
    public int? ReviewerEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the required documents for this milestone.
    /// </summary>
    [StringLength(1000)]
    public string? RequiredDocuments { get; set; }

    /// <summary>
    /// Gets or sets the required evidence or materials.
    /// </summary>
    [StringLength(1000)]
    public string? RequiredEvidence { get; set; }

    /// <summary>
    /// Gets or sets the evaluation criteria.
    /// </summary>
    [StringLength(2000)]
    public string? EvaluationCriteria { get; set; }

    /// <summary>
    /// Gets or sets the success criteria for this milestone.
    /// </summary>
    [StringLength(1000)]
    public string? SuccessCriteria { get; set; }

    /// <summary>
    /// Gets or sets the outcome of this milestone.
    /// </summary>
    [StringLength(50)]
    public string? Outcome { get; set; } // Satisfactory, Needs Improvement, Unsatisfactory, Excellent

    /// <summary>
    /// Gets or sets the outcome date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? OutcomeDate { get; set; }

    /// <summary>
    /// Gets or sets the detailed feedback.
    /// </summary>
    [StringLength(2000)]
    public string? Feedback { get; set; }

    /// <summary>
    /// Gets or sets recommendations for improvement.
    /// </summary>
    [StringLength(1000)]
    public string? Recommendations { get; set; }

    /// <summary>
    /// Gets or sets development goals arising from this milestone.
    /// </summary>
    [StringLength(1000)]
    public string? DevelopmentGoals { get; set; }

    /// <summary>
    /// Gets or sets whether this milestone impacts tenure eligibility.
    /// </summary>
    public bool ImpactsTenureEligibility { get; set; } = false;

    /// <summary>
    /// Gets or sets whether remediation is required.
    /// </summary>
    public bool RemediationRequired { get; set; } = false;

    /// <summary>
    /// Gets or sets the remediation plan.
    /// </summary>
    [StringLength(1000)]
    public string? RemediationPlan { get; set; }

    /// <summary>
    /// Gets or sets the remediation deadline.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? RemediationDeadline { get; set; }

    /// <summary>
    /// Gets or sets whether external review is required.
    /// </summary>
    public bool RequiresExternalReview { get; set; } = false;

    /// <summary>
    /// Gets or sets external reviewer information.
    /// </summary>
    [StringLength(500)]
    public string? ExternalReviewerInfo { get; set; }

    /// <summary>
    /// Gets or sets the notification requirements.
    /// </summary>
    [StringLength(200)]
    public string? NotificationRequirements { get; set; }

    /// <summary>
    /// Gets or sets the follow-up actions required.
    /// </summary>
    [StringLength(1000)]
    public string? FollowUpActions { get; set; }

    /// <summary>
    /// Gets or sets the next milestone dependencies.
    /// </summary>
    [StringLength(200)]
    public string? NextMilestoneDependencies { get; set; }

    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    [StringLength(1000)]
    public string? Metadata { get; set; }

    /// <summary>
    /// Gets or sets notes about this milestone.
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the tenure track.
    /// </summary>
    [ForeignKey(nameof(TenureTrackId))]
    public TenureTrack TenureTrack { get; set; } = null!;

    /// <summary>
    /// Gets or sets the reviewer if applicable.
    /// </summary>
    [ForeignKey(nameof(ReviewerEmpNr))]
    public Academic? Reviewer { get; set; }

    // Computed properties
    /// <summary>
    /// Gets whether this milestone is completed.
    /// </summary>
    [NotMapped]
    public bool IsCompleted => Status == "Completed";

    /// <summary>
    /// Gets whether this milestone is overdue.
    /// </summary>
    [NotMapped]
    public bool IsOverdue => ScheduledDate < DateTime.Today && !IsCompleted;

    /// <summary>
    /// Gets whether this milestone is coming up soon.
    /// </summary>
    [NotMapped]
    public bool IsUpcoming => ScheduledDate <= DateTime.Today.AddDays(30) &&
                             ScheduledDate >= DateTime.Today &&
                             !IsCompleted;

    /// <summary>
    /// Gets the number of days until this milestone.
    /// </summary>
    [NotMapped]
    public int DaysUntilMilestone => (int)(ScheduledDate - DateTime.Today).TotalDays;

    /// <summary>
    /// Gets the number of days overdue.
    /// </summary>
    [NotMapped]
    public int DaysOverdue => IsOverdue ? (int)(DateTime.Today - ScheduledDate).TotalDays : 0;

    /// <summary>
    /// Gets the preparation start date.
    /// </summary>
    [NotMapped]
    public DateTime PreparationStartDate => ScheduledDate.AddDays(-PreparationDays);

    /// <summary>
    /// Gets whether preparation should begin.
    /// </summary>
    [NotMapped]
    public bool ShouldBeginPreparation => DateTime.Today >= PreparationStartDate && !IsCompleted;

    /// <summary>
    /// Gets the milestone urgency level.
    /// </summary>
    [NotMapped]
    public string UrgencyLevel
    {
        get
        {
            if (IsOverdue) return "Critical";
            if (DaysUntilMilestone <= 7) return "Urgent";
            if (DaysUntilMilestone <= 14) return "High";
            if (DaysUntilMilestone <= 30) return "Medium";
            return "Low";
        }
    }

    /// <summary>
    /// Gets the milestone health status.
    /// </summary>
    [NotMapped]
    public string HealthStatus
    {
        get
        {
            if (IsCompleted && Outcome == "Excellent") return "Excellent";
            if (IsCompleted && Outcome == "Satisfactory") return "Good";
            if (IsCompleted && Outcome == "Needs Improvement") return "Concerning";
            if (IsCompleted && Outcome == "Unsatisfactory") return "Poor";
            if (IsCompleted) return "Completed";
            if (IsOverdue) return "Overdue";
            if (RemediationRequired) return "Remediation Required";
            if (IsUpcoming) return "Upcoming";
            return "On Track";
        }
    }

    /// <summary>
    /// Gets the full milestone identifier.
    /// </summary>
    [NotMapped]
    public string MilestoneIdentifier => $"Year {TenureYear} - {MilestoneName}";

    /// <summary>
    /// Gets whether outcome is satisfactory.
    /// </summary>
    [NotMapped]
    public bool HasSatisfactoryOutcome => Outcome == "Satisfactory" || Outcome == "Excellent";

    /// <summary>
    /// Gets whether this milestone requires immediate attention.
    /// </summary>
    [NotMapped]
    public bool RequiresImmediateAttention => IsOverdue ||
                                             RemediationRequired ||
                                             (IsUpcoming && ImportanceLevel == "Critical");

    /// <summary>
    /// Gets the completion percentage.
    /// </summary>
    [NotMapped]
    public decimal CompletionPercentage
    {
        get
        {
            return Status switch
            {
                "Completed" => 100m,
                "InProgress" => 50m,
                "Scheduled" when ShouldBeginPreparation => 25m,
                _ => 0m
            };
        }
    }
}