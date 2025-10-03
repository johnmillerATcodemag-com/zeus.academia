using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a catalog publication.
/// </summary>
public class CatalogPublication : BaseEntity
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
    /// Gets or sets the publication format.
    /// </summary>
    [Required]
    public PublicationFormat Format { get; set; }

    /// <summary>
    /// Gets or sets the publication title.
    /// </summary>
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the publication status.
    /// </summary>
    [Required]
    public PublicationStatus Status { get; set; } = PublicationStatus.Draft;

    /// <summary>
    /// Gets or sets the file path for the published catalog.
    /// </summary>
    [MaxLength(500)]
    public string? FilePath { get; set; }

    /// <summary>
    /// Gets or sets the URL where the catalog can be accessed.
    /// </summary>
    [MaxLength(500)]
    public string? AccessUrl { get; set; }

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// Gets or sets the number of pages.
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    [Required]
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the publication date.
    /// </summary>
    public DateTime? PublicationDate { get; set; }

    /// <summary>
    /// Gets or sets the last updated date.
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets who created this publication.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public new string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets who published this catalog.
    /// </summary>
    [MaxLength(100)]
    public string? PublishedBy { get; set; }

    /// <summary>
    /// Gets or sets the expiration date for this publication.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets whether this publication is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the download count.
    /// </summary>
    public int DownloadCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the view count.
    /// </summary>
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the distribution channels.
    /// </summary>
    public List<DistributionChannel> DistributionChannels { get; set; } = new();

    /// <summary>
    /// Gets or sets the security settings.
    /// </summary>
    public Dictionary<string, string> SecuritySettings { get; set; } = new();

    /// <summary>
    /// Gets or sets the metadata for this publication.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets publication settings/configuration.
    /// </summary>
    public Dictionary<string, object> PublicationSettings { get; set; } = new();

    /// <summary>
    /// Gets or sets the checksum for file integrity.
    /// </summary>
    [MaxLength(100)]
    public string? Checksum { get; set; }

    /// <summary>
    /// Gets or sets the version of this publication.
    /// </summary>
    [MaxLength(10)]
    public string Version { get; set; } = "1.0";

    // Navigation Properties

    /// <summary>
    /// Navigation property to the catalog.
    /// </summary>
    [ForeignKey(nameof(CatalogId))]
    public virtual CourseCatalog Catalog { get; set; } = null!;

    /// <summary>
    /// Navigation property to distribution records.
    /// </summary>
    public virtual ICollection<PublicationDistribution> Distributions { get; set; } = new List<PublicationDistribution>();

    /// <summary>
    /// Navigation property to access logs.
    /// </summary>
    public virtual ICollection<PublicationAccessLog> AccessLogs { get; set; } = new List<PublicationAccessLog>();
}

/// <summary>
/// Entity representing a publication distribution record.
/// </summary>
public class PublicationDistribution : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the catalog publication ID.
    /// </summary>
    [Required]
    public int CatalogPublicationId { get; set; }

    /// <summary>
    /// Gets or sets the distribution channel.
    /// </summary>
    [Required]
    public DistributionChannel Channel { get; set; }

    /// <summary>
    /// Gets or sets the distribution date.
    /// </summary>
    [Required]
    public DateTime DistributionDate { get; set; }

    /// <summary>
    /// Gets or sets the distribution status.
    /// </summary>
    [Required]
    public DistributionStatus Status { get; set; } = DistributionStatus.Pending;

    /// <summary>
    /// Gets or sets who initiated the distribution.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DistributedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target audience.
    /// </summary>
    [MaxLength(200)]
    public string? TargetAudience { get; set; }

    /// <summary>
    /// Gets or sets the number of recipients.
    /// </summary>
    public int? RecipientCount { get; set; }

    /// <summary>
    /// Gets or sets the delivery confirmation date.
    /// </summary>
    public DateTime? DeliveryConfirmationDate { get; set; }

    /// <summary>
    /// Gets or sets any error messages from distribution.
    /// </summary>
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the retry count for failed distributions.
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the next retry date.
    /// </summary>
    public DateTime? NextRetryDate { get; set; }

    /// <summary>
    /// Gets or sets distribution metrics.
    /// </summary>
    public Dictionary<string, decimal> Metrics { get; set; } = new();

    // Navigation Properties

    /// <summary>
    /// Navigation property to the catalog publication.
    /// </summary>
    [ForeignKey(nameof(CatalogPublicationId))]
    public virtual CatalogPublication CatalogPublication { get; set; } = null!;
}

/// <summary>
/// Entity representing an access log entry for a publication.
/// </summary>
public class PublicationAccessLog : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the catalog publication ID.
    /// </summary>
    [Required]
    public int CatalogPublicationId { get; set; }

    /// <summary>
    /// Gets or sets the access date and time.
    /// </summary>
    [Required]
    public DateTime AccessDateTime { get; set; }

    /// <summary>
    /// Gets or sets the access type (view, download, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string AccessType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets who accessed the publication.
    /// </summary>
    [MaxLength(100)]
    public string? AccessedBy { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the accessor.
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent string.
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the referrer URL.
    /// </summary>
    [MaxLength(500)]
    public string? Referrer { get; set; }

    /// <summary>
    /// Gets or sets the session ID.
    /// </summary>
    [MaxLength(100)]
    public string? SessionId { get; set; }

    /// <summary>
    /// Gets or sets the duration of access in seconds.
    /// </summary>
    public int? AccessDurationSeconds { get; set; }

    /// <summary>
    /// Gets or sets whether the access was successful.
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// Gets or sets any error message if access failed.
    /// </summary>
    [MaxLength(500)]
    public string? ErrorMessage { get; set; }

    // Navigation Properties

    /// <summary>
    /// Navigation property to the catalog publication.
    /// </summary>
    [ForeignKey(nameof(CatalogPublicationId))]
    public virtual CatalogPublication CatalogPublication { get; set; } = null!;
}