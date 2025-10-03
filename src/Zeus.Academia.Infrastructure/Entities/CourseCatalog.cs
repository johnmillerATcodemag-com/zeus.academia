using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a course catalog for a specific academic year.
/// Supports versioning, approval workflow, and publication management.
/// </summary>
public class CourseCatalog : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the academic year for this catalog.
    /// </summary>
    [Required]
    [Range(2020, 2050, ErrorMessage = "Academic year must be between 2020 and 2050")]
    public int AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the catalog name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CatalogName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the catalog version.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Gets or sets the catalog status.
    /// </summary>
    [Required]
    public CatalogStatus Status { get; set; } = CatalogStatus.Draft;

    /// <summary>
    /// Gets or sets the catalog type.
    /// </summary>
    [Required]
    public CatalogType CatalogType { get; set; } = CatalogType.Undergraduate;

    /// <summary>
    /// Gets or sets the catalog description.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the effective date when this catalog becomes active.
    /// </summary>
    [Required]
    [Column(TypeName = "date")]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets the expiration date when this catalog becomes inactive.
    /// </summary>
    [Required]
    [Column(TypeName = "date")]
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the publication date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? PublicationDate { get; set; }

    /// <summary>
    /// Gets or sets the last updated date.
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets whether this catalog is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the cover image URL.
    /// </summary>
    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages in the catalog.
    /// </summary>
    public int? TotalPages { get; set; }

    /// <summary>
    /// Gets or sets the total number of courses in the catalog.
    /// </summary>
    public int? TotalCourses { get; set; }

    /// <summary>
    /// Gets or sets the download count for this catalog.
    /// </summary>
    public int DownloadCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the view count for this catalog.
    /// </summary>
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the ID of the catalog this one is based on.
    /// </summary>
    public int? BasedOnCatalogId { get; set; }

    /// <summary>
    /// Gets or sets the publication formats available for this catalog.
    /// </summary>
    public List<PublicationFormat> PublicationFormats { get; set; } = new();

    /// <summary>
    /// Gets or sets the distribution channels for this catalog.
    /// </summary>
    public List<DistributionChannel> DistributionChannels { get; set; } = new();

    // Navigation Properties

    /// <summary>
    /// Navigation property to the catalog this one is based on.
    /// </summary>
    [ForeignKey(nameof(BasedOnCatalogId))]
    public virtual CourseCatalog? BasedOnCatalog { get; set; }

    /// <summary>
    /// Navigation property to catalogs based on this one.
    /// </summary>
    public virtual ICollection<CourseCatalog> DerivedCatalogs { get; set; } = new List<CourseCatalog>();

    /// <summary>
    /// Navigation property to course approval workflows in this catalog.
    /// </summary>
    public virtual ICollection<CourseApprovalWorkflow> ApprovalWorkflows { get; set; } = new List<CourseApprovalWorkflow>();

    /// <summary>
    /// Navigation property to catalog versions.
    /// </summary>
    public virtual ICollection<CatalogVersion> Versions { get; set; } = new List<CatalogVersion>();

    /// <summary>
    /// Navigation property to catalog publications.
    /// </summary>
    public virtual ICollection<CatalogPublication> Publications { get; set; } = new List<CatalogPublication>();

    /// <summary>
    /// Navigation property to catalog approval history.
    /// </summary>
    public virtual ICollection<CatalogApproval> ApprovalHistory { get; set; } = new List<CatalogApproval>();

    /// <summary>
    /// Navigation property to courses in this catalog.
    /// </summary>
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}

/// <summary>
/// Entity representing a course approval workflow instance.
/// </summary>
public class CourseApprovalWorkflow : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the course ID being approved.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the catalog ID this workflow belongs to.
    /// </summary>
    public int? CatalogId { get; set; }

    /// <summary>
    /// Gets or sets the workflow name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string WorkflowName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets who initiated the workflow.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string InitiatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the initiation date.
    /// </summary>
    [Required]
    public DateTime InitiationDate { get; set; }

    /// <summary>
    /// Gets or sets the workflow status.
    /// </summary>
    [Required]
    public WorkflowStatus Status { get; set; } = WorkflowStatus.NotStarted;

    /// <summary>
    /// Gets or sets the current approval stage.
    /// </summary>
    [Required]
    public ApprovalStage CurrentStage { get; set; } = ApprovalStage.DepartmentReview;

    /// <summary>
    /// Gets or sets the completion date.
    /// </summary>
    public DateTime? CompletionDate { get; set; }

    /// <summary>
    /// Gets or sets workflow notes.
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the expected completion date.
    /// </summary>
    public DateTime? ExpectedCompletionDate { get; set; }

    /// <summary>
    /// Gets or sets the priority level.
    /// </summary>
    [MaxLength(20)]
    public string Priority { get; set; } = "Normal";

    // Navigation Properties

    /// <summary>
    /// Navigation property to the course being approved.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;

    /// <summary>
    /// Navigation property to the catalog.
    /// </summary>
    [ForeignKey(nameof(CatalogId))]
    public virtual CourseCatalog? Catalog { get; set; }

    /// <summary>
    /// Navigation property to approval steps.
    /// </summary>
    public virtual ICollection<ApprovalStep> ApprovalSteps { get; set; } = new List<ApprovalStep>();
}

/// <summary>
/// Entity representing an individual approval step within a workflow.
/// </summary>
public class ApprovalStep : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the workflow ID this step belongs to.
    /// </summary>
    [Required]
    public int WorkflowId { get; set; }

    /// <summary>
    /// Gets or sets the step order in the workflow.
    /// </summary>
    [Required]
    public int StepOrder { get; set; }

    /// <summary>
    /// Gets or sets the approval stage.
    /// </summary>
    [Required]
    public ApprovalStage ApprovalStage { get; set; }

    /// <summary>
    /// Gets or sets who this step is assigned to.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string AssignedTo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the step status.
    /// </summary>
    [Required]
    public ApprovalStatus Status { get; set; } = ApprovalStatus.NotStarted;

    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the completion date.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the due date.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the estimated duration.
    /// </summary>
    public TimeSpan EstimatedDuration { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    /// Gets or sets the actual duration.
    /// </summary>
    public TimeSpan? ActualDuration { get; set; }

    /// <summary>
    /// Gets or sets comments from the reviewer.
    /// </summary>
    [MaxLength(2000)]
    public string? Comments { get; set; }

    /// <summary>
    /// Gets or sets the required documents for this step.
    /// </summary>
    public List<string> RequiredDocuments { get; set; } = new();

    /// <summary>
    /// Gets or sets the review criteria for this step.
    /// </summary>
    public List<string> ReviewCriteria { get; set; } = new();

    // Navigation Properties

    /// <summary>
    /// Navigation property to the parent workflow.
    /// </summary>
    [ForeignKey(nameof(WorkflowId))]
    public virtual CourseApprovalWorkflow Workflow { get; set; } = null!;

    /// <summary>
    /// Navigation property to approval attachments.
    /// </summary>
    public virtual ICollection<ApprovalAttachment> Attachments { get; set; } = new List<ApprovalAttachment>();
}

/// <summary>
/// Entity representing an attachment in an approval step.
/// </summary>
public class ApprovalAttachment : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>  
    /// Gets or sets the approval step ID.
    /// </summary>
    [Required]
    public int ApprovalStepId { get; set; }

    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file path.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the upload date.
    /// </summary>
    [Required]
    public DateTime UploadDate { get; set; }

    /// <summary>
    /// Gets or sets who uploaded the file.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UploadedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    // Navigation Properties

    /// <summary>
    /// Navigation property to the approval step.
    /// </summary>
    [ForeignKey(nameof(ApprovalStepId))]
    public virtual ApprovalStep ApprovalStep { get; set; } = null!;
}

/// <summary>
/// Entity representing a catalog approval record.
/// </summary>
public class CatalogApproval : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the catalog ID.
    /// </summary>
    [Required]
    public int CatalogId { get; set; }

    /// <summary>
    /// Gets or sets the approval stage.
    /// </summary>
    [Required]
    public ApprovalStage ApprovalStage { get; set; }

    /// <summary>
    /// Gets or sets who approved.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the approval date.
    /// </summary>
    [Required]
    public DateTime ApprovalDate { get; set; }

    /// <summary>
    /// Gets or sets the approval status.
    /// </summary>
    [Required]
    public ApprovalStatus Status { get; set; }

    /// <summary>
    /// Gets or sets approval comments.
    /// </summary>
    [MaxLength(1000)]
    public string? Comments { get; set; }

    // Navigation Properties

    /// <summary>
    /// Navigation property to the catalog.
    /// </summary>
    [ForeignKey(nameof(CatalogId))]
    public virtual CourseCatalog Catalog { get; set; } = null!;
}