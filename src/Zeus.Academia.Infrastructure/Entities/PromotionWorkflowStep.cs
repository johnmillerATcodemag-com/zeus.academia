using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a step in the promotion workflow process.
/// Manages the sequential stages of promotion review and approval.
/// </summary>
public class PromotionWorkflowStep : BaseEntity
{
    /// <summary>
    /// Gets or sets the promotion application ID.
    /// </summary>
    [Required]
    public int PromotionApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the step sequence number in the workflow.
    /// </summary>
    [Required]
    public int StepOrder { get; set; }

    /// <summary>
    /// Gets or sets the name of the workflow step.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string StepName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of what this step entails.
    /// </summary>
    [StringLength(500)]
    public string? StepDescription { get; set; }

    /// <summary>
    /// Gets or sets the type of step in the workflow.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string StepType { get; set; } = string.Empty; // Review, Approval, Notification, Documentation

    /// <summary>
    /// Gets or sets the current status of this step.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Skipped, Failed

    /// <summary>
    /// Gets or sets the step start date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the step completion date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? CompletionDate { get; set; }

    /// <summary>
    /// Gets or sets the due date for this step.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the step duration in business days.
    /// </summary>
    public int EstimatedDurationDays { get; set; } = 30;

    /// <summary>
    /// Gets or sets the actual duration taken to complete this step.
    /// </summary>
    public int? ActualDurationDays { get; set; }

    /// <summary>
    /// Gets or sets the responsible party for this step.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ResponsibleParty { get; set; } = string.Empty; // Department, College, Committee, Provost, etc.

    /// <summary>
    /// Gets or sets the specific committee ID if applicable.
    /// </summary>
    public int? PromotionCommitteeId { get; set; }

    /// <summary>
    /// Gets or sets the employee number of the assigned reviewer.
    /// </summary>
    public int? AssignedReviewerEmpNr { get; set; }

    /// <summary>
    /// Gets or sets whether this step is mandatory.
    /// </summary>
    [Required]
    public bool IsMandatory { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this step can be performed in parallel with others.
    /// </summary>
    public bool CanRunInParallel { get; set; } = false;

    /// <summary>
    /// Gets or sets the prerequisite steps that must be completed first.
    /// </summary>
    [StringLength(200)]
    public string? PrerequisiteSteps { get; set; } // Comma-separated step orders

    /// <summary>
    /// Gets or sets the required documents for this step.
    /// </summary>
    [StringLength(500)]
    public string? RequiredDocuments { get; set; }

    /// <summary>
    /// Gets or sets the deliverables expected from this step.
    /// </summary>
    [StringLength(500)]
    public string? ExpectedDeliverables { get; set; }

    /// <summary>
    /// Gets or sets the approval criteria for this step.
    /// </summary>
    [StringLength(1000)]
    public string? ApprovalCriteria { get; set; }

    /// <summary>
    /// Gets or sets the outcome of this step.
    /// </summary>
    [StringLength(50)]
    public string? Outcome { get; set; } // Approved, Rejected, Deferred, More Info Needed

    /// <summary>
    /// Gets or sets the outcome date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? OutcomeDate { get; set; }

    /// <summary>
    /// Gets or sets the vote or score if applicable.
    /// </summary>
    [StringLength(50)]
    public string? VoteResult { get; set; }

    /// <summary>
    /// Gets or sets feedback or comments from this step.
    /// </summary>
    [StringLength(2000)]
    public string? Feedback { get; set; }

    /// <summary>
    /// Gets or sets recommendations for improvement.
    /// </summary>
    [StringLength(1000)]
    public string? Recommendations { get; set; }

    /// <summary>
    /// Gets or sets whether this step requires external review.
    /// </summary>
    public bool RequiresExternalReview { get; set; } = false;

    /// <summary>
    /// Gets or sets the external reviewer information.
    /// </summary>
    [StringLength(500)]
    public string? ExternalReviewerInfo { get; set; }

    /// <summary>
    /// Gets or sets the notification requirements for this step.
    /// </summary>
    [StringLength(200)]
    public string? NotificationRequirements { get; set; }

    /// <summary>
    /// Gets or sets the escalation path if step is delayed.
    /// </summary>
    [StringLength(200)]
    public string? EscalationPath { get; set; }

    /// <summary>
    /// Gets or sets the step completion percentage.
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal CompletionPercentage { get; set; } = 0.0m;

    /// <summary>
    /// Gets or sets additional metadata for this step.
    /// </summary>
    [StringLength(1000)]
    public string? StepMetadata { get; set; }

    /// <summary>
    /// Gets or sets notes about this step.
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
    /// Gets or sets the promotion committee if applicable.
    /// </summary>
    [ForeignKey(nameof(PromotionCommitteeId))]
    public PromotionCommittee? PromotionCommittee { get; set; }

    /// <summary>
    /// Gets or sets the assigned reviewer if applicable.
    /// </summary>
    [ForeignKey(nameof(AssignedReviewerEmpNr))]
    public Academic? AssignedReviewer { get; set; }

    // Computed properties
    /// <summary>
    /// Gets whether this step is currently active.
    /// </summary>
    [NotMapped]
    public bool IsActive => Status == "InProgress";

    /// <summary>
    /// Gets whether this step is completed.
    /// </summary>
    [NotMapped]
    public bool IsCompleted => Status == "Completed";

    /// <summary>
    /// Gets whether this step is overdue.
    /// </summary>
    [NotMapped]
    public bool IsOverdue => DueDate.HasValue &&
                            DueDate < DateTime.Today &&
                            !IsCompleted;

    /// <summary>
    /// Gets the number of days remaining until due date.
    /// </summary>
    [NotMapped]
    public int? DaysUntilDue => DueDate?.Subtract(DateTime.Today).Days;

    /// <summary>
    /// Gets the number of days this step has been active.
    /// </summary>
    [NotMapped]
    public int? DaysActive => StartDate?.Subtract(DateTime.Today).Days * -1;

    /// <summary>
    /// Gets whether this step is at risk of being late.
    /// </summary>
    [NotMapped]
    public bool IsAtRisk => DaysUntilDue.HasValue &&
                           DaysUntilDue <= 5 &&
                           DaysUntilDue > 0 &&
                           !IsCompleted;

    /// <summary>
    /// Gets the step's priority level based on due date and status.
    /// </summary>
    [NotMapped]
    public string PriorityLevel
    {
        get
        {
            if (IsOverdue) return "Critical";
            if (IsAtRisk) return "High";
            if (IsActive) return "Medium";
            return "Low";
        }
    }

    /// <summary>
    /// Gets the step's health status.
    /// </summary>
    [NotMapped]
    public string HealthStatus
    {
        get
        {
            if (IsCompleted) return "Completed";
            if (IsOverdue) return "Overdue";
            if (IsAtRisk) return "At Risk";
            if (IsActive) return "On Track";
            return "Pending";
        }
    }

    /// <summary>
    /// Gets the full step identifier.
    /// </summary>
    [NotMapped]
    public string StepIdentifier => $"Step {StepOrder}: {StepName}";

    /// <summary>
    /// Gets whether prerequisites are met.
    /// </summary>
    [NotMapped]
    public bool PrerequisitesMet
    {
        get
        {
            if (string.IsNullOrEmpty(PrerequisiteSteps)) return true;

            var prerequisiteStepOrders = PrerequisiteSteps
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var order) ? order : 0)
                .Where(order => order > 0);

            return PromotionApplication?.WorkflowSteps?
                .Where(step => prerequisiteStepOrders.Contains(step.StepOrder))
                .All(step => step.IsCompleted) ?? true;
        }
    }

    /// <summary>
    /// Gets whether this step can be started.
    /// </summary>
    [NotMapped]
    public bool CanStart => Status == "Pending" && PrerequisitesMet;
}