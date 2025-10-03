using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an administrative override of prerequisite requirements.
/// </summary>
public class PrerequisiteOverride : BaseEntity
{
    /// <summary>
    /// Gets or sets the student ID this override applies to.
    /// </summary>
    [Required]
    public int StudentId { get; set; }

    /// <summary>
    /// Gets or sets the course ID this override applies to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the academic term ID this override is valid for.
    /// </summary>
    [Required]
    public int AcademicTermId { get; set; }

    /// <summary>
    /// Gets or sets the type of override being granted.
    /// </summary>
    [Required]
    public OverrideType OverrideType { get; set; }

    /// <summary>
    /// Gets or sets the scope of the override.
    /// </summary>
    [Required]
    public OverrideScope OverrideScope { get; set; }

    /// <summary>
    /// Gets or sets the status of this override.
    /// </summary>
    [Required]
    public OverrideStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the reason for granting this override.
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string OverrideReason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets who requested this override.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string RequestedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when this override was requested.
    /// </summary>
    [Required]
    public DateTime RequestedDate { get; set; }

    /// <summary>
    /// Gets or sets who approved this override.
    /// </summary>
    [StringLength(100)]
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// Gets or sets when this override was approved.
    /// </summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Gets or sets the authority level of the approver.
    /// </summary>
    public AuthorityLevel? ApproverAuthority { get; set; }

    /// <summary>
    /// Gets or sets when this override expires.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets whether this override is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets any conditions attached to this override.
    /// </summary>
    [StringLength(1000)]
    public string? Conditions { get; set; }

    /// <summary>
    /// Gets or sets the priority of this override when multiple apply.
    /// </summary>
    public int Priority { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether this override requires periodic review.
    /// </summary>
    public bool RequiresPeriodicReview { get; set; } = false;

    /// <summary>
    /// Gets or sets the frequency of required reviews in days.
    /// </summary>
    public int? ReviewFrequencyDays { get; set; }

    /// <summary>
    /// Gets or sets when this override was last reviewed.
    /// </summary>
    public DateTime? LastReviewDate { get; set; }

    /// <summary>
    /// Gets or sets when this override is due for next review.
    /// </summary>
    public DateTime? NextReviewDate { get; set; }

    /// <summary>
    /// Gets or sets additional notes about this override.
    /// </summary>
    [StringLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets metadata about this override.
    /// </summary>
    [StringLength(1000)]
    public string? OverrideMetadata { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the student this override applies to.
    /// </summary>
    [ForeignKey(nameof(StudentId))]
    public Student Student { get; set; } = null!;

    /// <summary>
    /// Gets or sets the course this override applies to.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    /// <summary>
    /// Gets or sets the academic term this override is valid for.
    /// </summary>
    [ForeignKey(nameof(AcademicTermId))]
    public AcademicTerm AcademicTerm { get; set; } = null!;

    /// <summary>
    /// Gets or sets the specific rules this override affects.
    /// </summary>
    public List<OverrideRuleMapping> AffectedRules { get; set; } = new();

    /// <summary>
    /// Gets or sets the approval workflow for this override.
    /// </summary>
    public List<OverrideApprovalStep> ApprovalWorkflow { get; set; } = new();

    /// <summary>
    /// Gets or sets the audit trail for this override.
    /// </summary>
    public List<OverrideAuditEntry> AuditTrail { get; set; } = new();

    /// <summary>
    /// Gets or sets any attached documentation for this override.
    /// </summary>
    public List<OverrideDocument> AttachedDocuments { get; set; } = new();
}

/// <summary>
/// Entity representing a waiver of prerequisite requirements.
/// </summary>
public class PrerequisiteWaiver : BaseEntity
{
    /// <summary>
    /// Gets or sets the student ID this waiver applies to.
    /// </summary>
    [Required]
    public int StudentId { get; set; }

    /// <summary>
    /// Gets or sets the course ID this waiver applies to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the type of waiver being granted.
    /// </summary>
    [Required]
    public WaiverType WaiverType { get; set; }

    /// <summary>
    /// Gets or sets the scope of the waiver.
    /// </summary>
    [Required]
    public WaiverScope WaiverScope { get; set; }

    /// <summary>
    /// Gets or sets the status of this waiver.
    /// </summary>
    [Required]
    public WaiverStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the reason for granting this waiver.
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string WaiverReason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the justification for this waiver.
    /// </summary>
    [Required]
    [StringLength(2000)]
    public string Justification { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets who requested this waiver.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string RequestedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when this waiver was requested.
    /// </summary>
    [Required]
    public DateTime RequestedDate { get; set; }

    /// <summary>
    /// Gets or sets who approved this waiver.
    /// </summary>
    [StringLength(100)]
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// Gets or sets when this waiver was approved.
    /// </summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Gets or sets when this waiver expires.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets whether this waiver is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this waiver is permanent.
    /// </summary>
    public bool IsPermanent { get; set; } = false;

    /// <summary>
    /// Gets or sets any conditions attached to this waiver.
    /// </summary>
    [StringLength(1000)]
    public string? Conditions { get; set; }

    /// <summary>
    /// Gets or sets any academic consequences of this waiver.
    /// </summary>
    [StringLength(1000)]
    public string? AcademicConsequences { get; set; }

    /// <summary>
    /// Gets or sets whether the student acknowledged the consequences.
    /// </summary>
    public bool StudentAcknowledged { get; set; } = false;

    /// <summary>
    /// Gets or sets when the student acknowledged the waiver.
    /// </summary>
    public DateTime? AcknowledgmentDate { get; set; }

    /// <summary>
    /// Gets or sets additional notes about this waiver.
    /// </summary>
    [StringLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets metadata about this waiver.
    /// </summary>
    [StringLength(1000)]
    public string? WaiverMetadata { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the student this waiver applies to.
    /// </summary>
    [ForeignKey(nameof(StudentId))]
    public Student Student { get; set; } = null!;

    /// <summary>
    /// Gets or sets the course this waiver applies to.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    /// <summary>
    /// Gets or sets the specific rules this waiver affects.
    /// </summary>
    public List<WaiverRuleMapping> AffectedRules { get; set; } = new();

    /// <summary>
    /// Gets or sets any attached documentation for this waiver.
    /// </summary>
    public List<WaiverDocument> AttachedDocuments { get; set; } = new();
}

/// <summary>
/// Entity mapping specific rules affected by an override.
/// </summary>
public class OverrideRuleMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the prerequisite override ID.
    /// </summary>
    [Required]
    public int PrerequisiteOverrideId { get; set; }

    /// <summary>
    /// Gets or sets the prerequisite rule ID affected by the override.
    /// </summary>
    public int? PrerequisiteRuleId { get; set; }

    /// <summary>
    /// Gets or sets the corequisite rule ID affected by the override.
    /// </summary>
    public int? CorequisiteRuleId { get; set; }

    /// <summary>
    /// Gets or sets the enrollment restriction ID affected by the override.
    /// </summary>
    public int? EnrollmentRestrictionId { get; set; }

    /// <summary>
    /// Gets or sets whether this rule is completely overridden.
    /// </summary>
    public bool IsCompleteOverride { get; set; } = true;

    /// <summary>
    /// Gets or sets specific conditions for partial override.
    /// </summary>
    [StringLength(500)]
    public string? PartialOverrideConditions { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the prerequisite override this mapping belongs to.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteOverrideId))]
    public PrerequisiteOverride PrerequisiteOverride { get; set; } = null!;

    /// <summary>
    /// Gets or sets the prerequisite rule affected (if applicable).
    /// </summary>
    [ForeignKey(nameof(PrerequisiteRuleId))]
    public PrerequisiteRule? PrerequisiteRule { get; set; }

    /// <summary>
    /// Gets or sets the corequisite rule affected (if applicable).
    /// </summary>
    [ForeignKey(nameof(CorequisiteRuleId))]
    public CorequisiteRule? CorequisiteRule { get; set; }

    /// <summary>
    /// Gets or sets the enrollment restriction affected (if applicable).
    /// </summary>
    [ForeignKey(nameof(EnrollmentRestrictionId))]
    public EnrollmentRestriction? EnrollmentRestriction { get; set; }
}

/// <summary>
/// Entity mapping specific rules affected by a waiver.
/// </summary>
public class WaiverRuleMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the prerequisite waiver ID.
    /// </summary>
    [Required]
    public int PrerequisiteWaiverId { get; set; }

    /// <summary>
    /// Gets or sets the prerequisite rule ID affected by the waiver.
    /// </summary>
    public int? PrerequisiteRuleId { get; set; }

    /// <summary>
    /// Gets or sets the corequisite rule ID affected by the waiver.
    /// </summary>
    public int? CorequisiteRuleId { get; set; }

    /// <summary>
    /// Gets or sets whether this rule is completely waived.
    /// </summary>
    public bool IsCompleteWaiver { get; set; } = true;

    /// <summary>
    /// Gets or sets specific conditions for partial waiver.
    /// </summary>
    [StringLength(500)]
    public string? PartialWaiverConditions { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the prerequisite waiver this mapping belongs to.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteWaiverId))]
    public PrerequisiteWaiver PrerequisiteWaiver { get; set; } = null!;

    /// <summary>
    /// Gets or sets the prerequisite rule affected (if applicable).
    /// </summary>
    [ForeignKey(nameof(PrerequisiteRuleId))]
    public PrerequisiteRule? PrerequisiteRule { get; set; }

    /// <summary>
    /// Gets or sets the corequisite rule affected (if applicable).
    /// </summary>
    [ForeignKey(nameof(CorequisiteRuleId))]
    public CorequisiteRule? CorequisiteRule { get; set; }
}

/// <summary>
/// Entity representing steps in the override approval workflow.
/// </summary>
public class OverrideApprovalStep : BaseEntity
{
    /// <summary>
    /// Gets or sets the prerequisite override ID this step belongs to.
    /// </summary>
    [Required]
    public int PrerequisiteOverrideId { get; set; }

    /// <summary>
    /// Gets or sets the step number in the approval workflow.
    /// </summary>
    [Required]
    public int StepNumber { get; set; }

    /// <summary>
    /// Gets or sets the name of this approval step.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string StepName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of this approval step.
    /// </summary>
    [Required]
    public ApprovalStepStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the required authority level for this step.
    /// </summary>
    [Required]
    public AuthorityLevel RequiredAuthority { get; set; }

    /// <summary>
    /// Gets or sets who should approve this step.
    /// </summary>
    [StringLength(100)]
    public string? AssignedTo { get; set; }

    /// <summary>
    /// Gets or sets who actually approved this step.
    /// </summary>
    [StringLength(100)]
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// Gets or sets when this step was approved.
    /// </summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Gets or sets comments from the approver.
    /// </summary>
    [StringLength(1000)]
    public string? ApproverComments { get; set; }

    /// <summary>
    /// Gets or sets when this step is due for approval.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Gets or sets whether this step can be delegated.
    /// </summary>
    public bool CanDelegate { get; set; } = false;

    /// <summary>
    /// Gets or sets who this step was delegated to.
    /// </summary>
    [StringLength(100)]
    public string? DelegatedTo { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the prerequisite override this step belongs to.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteOverrideId))]
    public PrerequisiteOverride PrerequisiteOverride { get; set; } = null!;
}

/// <summary>
/// Entity representing audit trail entries for overrides.
/// </summary>
public class OverrideAuditEntry : BaseEntity
{
    /// <summary>
    /// Gets or sets the prerequisite override ID this audit entry belongs to.
    /// </summary>
    [Required]
    public int PrerequisiteOverrideId { get; set; }

    /// <summary>
    /// Gets or sets when this audit event occurred.
    /// </summary>
    [Required]
    public DateTime AuditDate { get; set; }

    /// <summary>
    /// Gets or sets who performed the audited action.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string PerformedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of action performed.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the action performed.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string ActionDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the old values before the action (if applicable).
    /// </summary>
    [StringLength(2000)]
    public string? OldValues { get; set; }

    /// <summary>
    /// Gets or sets the new values after the action (if applicable).
    /// </summary>
    [StringLength(2000)]
    public string? NewValues { get; set; }

    /// <summary>
    /// Gets or sets the IP address from which the action was performed.
    /// </summary>
    [StringLength(45)]
    public string? IPAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent information.
    /// </summary>
    [StringLength(500)]
    public string? UserAgent { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the prerequisite override this audit entry belongs to.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteOverrideId))]
    public PrerequisiteOverride PrerequisiteOverride { get; set; } = null!;
}

/// <summary>
/// Entity representing documents attached to overrides.
/// </summary>
public class OverrideDocument : BaseEntity
{
    /// <summary>
    /// Gets or sets the prerequisite override ID this document belongs to.
    /// </summary>
    [Required]
    public int PrerequisiteOverrideId { get; set; }

    /// <summary>
    /// Gets or sets the original filename of the document.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content type of the document.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size of the document in bytes.
    /// </summary>
    [Required]
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the storage path for the document.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the document.
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets who uploaded the document.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string UploadedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the document was uploaded.
    /// </summary>
    [Required]
    public DateTime UploadedDate { get; set; }

    /// <summary>
    /// Gets or sets whether the document is required for the override.
    /// </summary>
    public bool IsRequired { get; set; } = false;

    // Navigation properties
    /// <summary>
    /// Gets or sets the prerequisite override this document belongs to.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteOverrideId))]
    public PrerequisiteOverride PrerequisiteOverride { get; set; } = null!;
}

/// <summary>
/// Entity representing documents attached to waivers.
/// </summary>
public class WaiverDocument : BaseEntity
{
    /// <summary>
    /// Gets or sets the prerequisite waiver ID this document belongs to.
    /// </summary>
    [Required]
    public int PrerequisiteWaiverId { get; set; }

    /// <summary>
    /// Gets or sets the original filename of the document.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content type of the document.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size of the document in bytes.
    /// </summary>
    [Required]
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the storage path for the document.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the document.
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets who uploaded the document.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string UploadedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the document was uploaded.
    /// </summary>
    [Required]
    public DateTime UploadedDate { get; set; }

    /// <summary>
    /// Gets or sets whether the document is required for the waiver.
    /// </summary>
    public bool IsRequired { get; set; } = false;

    // Navigation properties
    /// <summary>
    /// Gets or sets the prerequisite waiver this document belongs to.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteWaiverId))]
    public PrerequisiteWaiver PrerequisiteWaiver { get; set; } = null!;
}