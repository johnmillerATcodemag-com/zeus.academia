using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a catalog version for tracking changes and history.
/// </summary>
public class CatalogVersion : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the catalog ID this version belongs to.
    /// </summary>
    [Required]
    public int CatalogId { get; set; }

    /// <summary>
    /// Gets or sets the version number.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string VersionNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version type.
    /// </summary>
    [Required]
    public VersionType VersionType { get; set; } = VersionType.Minor;

    /// <summary>
    /// Gets or sets the version name/title.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string VersionName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version description.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets when this version was created.
    /// </summary>
    [Required]
    public new DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets who created this version.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public new string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this is the current/active version.
    /// </summary>
    public bool IsCurrent { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this version is published.
    /// </summary>
    public bool IsPublished { get; set; } = false;

    /// <summary>
    /// Gets or sets the publication date.
    /// </summary>
    public DateTime? PublishedDate { get; set; }

    /// <summary>
    /// Gets or sets who published this version.
    /// </summary>
    [MaxLength(100)]
    public string? PublishedBy { get; set; }

    /// <summary>
    /// Gets or sets the ID of the previous version.
    /// </summary>
    public int? PreviousVersionId { get; set; }

    /// <summary>
    /// Gets or sets the snapshot data for this version.
    /// </summary>
    public string? SnapshotData { get; set; }

    /// <summary>
    /// Gets or sets the file size of the snapshot.
    /// </summary>
    public long? SnapshotSize { get; set; }

    /// <summary>
    /// Gets or sets the change summary.
    /// </summary>
    [MaxLength(2000)]
    public string? ChangeSummary { get; set; }

    /// <summary>
    /// Gets or sets the change log.
    /// </summary>
    public List<string> ChangeLog { get; set; } = new();

    /// <summary>
    /// Gets or sets the release notes.
    /// </summary>
    [MaxLength(5000)]
    public string? ReleaseNotes { get; set; }

    /// <summary>
    /// Gets or sets version tags.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets version metadata.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets the approval status for this version.
    /// </summary>
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.NotStarted;

    /// <summary>
    /// Gets or sets when this version was approved.
    /// </summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Gets or sets who approved this version.
    /// </summary>
    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// Gets or sets the expiration date for this version.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets whether this version is archived.
    /// </summary>
    public bool IsArchived { get; set; } = false;

    /// <summary>
    /// Gets or sets the archive date.
    /// </summary>
    public DateTime? ArchivedDate { get; set; }

    // Navigation Properties

    /// <summary>
    /// Navigation property to the catalog.
    /// </summary>
    [ForeignKey(nameof(CatalogId))]
    public virtual CourseCatalog Catalog { get; set; } = null!;

    /// <summary>
    /// Navigation property to the previous version.
    /// </summary>
    [ForeignKey(nameof(PreviousVersionId))]
    public virtual CatalogVersion? PreviousVersion { get; set; }

    /// <summary>
    /// Navigation property to subsequent versions.
    /// </summary>
    public virtual ICollection<CatalogVersion> SubsequentVersions { get; set; } = new List<CatalogVersion>();

    /// <summary>
    /// Navigation property to version changes.
    /// </summary>
    public virtual ICollection<VersionChange> Changes { get; set; } = new List<VersionChange>();

    /// <summary>
    /// Navigation property to version comparisons.
    /// </summary>
    public virtual ICollection<VersionComparison> ComparisonsAsSource { get; set; } = new List<VersionComparison>();

    /// <summary>
    /// Navigation property to version comparisons.
    /// </summary>
    public virtual ICollection<VersionComparison> ComparisonsAsTarget { get; set; } = new List<VersionComparison>();
}

/// <summary>
/// Entity representing a change within a catalog version.
/// </summary>
public class VersionChange : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the catalog version ID.
    /// </summary>
    [Required]
    public int CatalogVersionId { get; set; }

    /// <summary>
    /// Gets or sets the change type.
    /// </summary>
    [Required]
    public ChangeType ChangeType { get; set; }

    /// <summary>
    /// Gets or sets the entity type that was changed.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entity ID that was changed.
    /// </summary>
    [Required]
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets the property/field that was changed.
    /// </summary>
    [MaxLength(100)]
    public string? PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the old value.
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Gets or sets the new value.
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the change description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets when the change was made.
    /// </summary>
    [Required]
    public DateTime ChangeDate { get; set; }

    /// <summary>
    /// Gets or sets who made the change.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ChangedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the impact level of this change.
    /// </summary>
    [MaxLength(20)]
    public string ImpactLevel { get; set; } = "Low";

    /// <summary>
    /// Gets or sets whether this change requires approval.
    /// </summary>
    public bool RequiresApproval { get; set; } = false;

    /// <summary>
    /// Gets or sets the approval status for this change.
    /// </summary>
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.NotStarted;

    /// <summary>
    /// Gets or sets additional change metadata.
    /// </summary>
    public Dictionary<string, string> ChangeMetadata { get; set; } = new();

    // Navigation Properties

    /// <summary>
    /// Navigation property to the catalog version.
    /// </summary>
    [ForeignKey(nameof(CatalogVersionId))]
    public virtual CatalogVersion CatalogVersion { get; set; } = null!;
}

/// <summary>
/// Entity representing a comparison between two catalog versions.
/// </summary>
public class VersionComparison : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the source version ID.
    /// </summary>
    [Required]
    public int SourceVersionId { get; set; }

    /// <summary>
    /// Gets or sets the target version ID.
    /// </summary>
    [Required]
    public int TargetVersionId { get; set; }

    /// <summary>
    /// Gets or sets the comparison type.
    /// </summary>
    [Required]
    public ComparisonType ComparisonType { get; set; } = ComparisonType.Diff;

    /// <summary>
    /// Gets or sets when the comparison was performed.
    /// </summary>
    [Required]
    public DateTime ComparisonDate { get; set; }

    /// <summary>
    /// Gets or sets who performed the comparison.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ComparedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the comparison results.
    /// </summary>
    public string? ComparisonResults { get; set; }

    /// <summary>
    /// Gets or sets the summary of differences.
    /// </summary>
    [MaxLength(2000)]
    public string? DifferencesSummary { get; set; }

    /// <summary>
    /// Gets or sets the number of additions.
    /// </summary>
    public int AdditionsCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of modifications.
    /// </summary>
    public int ModificationsCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of deletions.
    /// </summary>
    public int DeletionsCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets comparison metrics.
    /// </summary>
    public Dictionary<string, decimal> ComparisonMetrics { get; set; } = new();

    /// <summary>
    /// Gets or sets the similarity percentage.
    /// </summary>
    [Range(0, 100)]
    public decimal SimilarityPercentage { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether this comparison is archived.
    /// </summary>
    public bool IsArchived { get; set; } = false;

    // Navigation Properties

    /// <summary>
    /// Navigation property to the source version.
    /// </summary>
    [ForeignKey(nameof(SourceVersionId))]
    public virtual CatalogVersion SourceVersion { get; set; } = null!;

    /// <summary>
    /// Navigation property to the target version.
    /// </summary>
    [ForeignKey(nameof(TargetVersionId))]
    public virtual CatalogVersion TargetVersion { get; set; } = null!;

    /// <summary>
    /// Navigation property to detailed differences.
    /// </summary>
    public virtual ICollection<ComparisonDetail> Details { get; set; } = new List<ComparisonDetail>();
}

/// <summary>
/// Entity representing detailed comparison information.
/// </summary>
public class ComparisonDetail : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the version comparison ID.
    /// </summary>
    [Required]
    public int VersionComparisonId { get; set; }

    /// <summary>
    /// Gets or sets the entity type being compared.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entity ID.
    /// </summary>
    [Required]
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets the property being compared.
    /// </summary>
    [MaxLength(100)]
    public string? PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the change type for this detail.
    /// </summary>
    [Required]
    public ChangeType ChangeType { get; set; }

    /// <summary>
    /// Gets or sets the old value.
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Gets or sets the new value.
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the difference description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the significance of this difference.
    /// </summary>
    [MaxLength(20)]
    public string Significance { get; set; } = "Low";

    // Navigation Properties

    /// <summary>
    /// Navigation property to the version comparison.
    /// </summary>
    [ForeignKey(nameof(VersionComparisonId))]
    public virtual VersionComparison VersionComparison { get; set; } = null!;
}