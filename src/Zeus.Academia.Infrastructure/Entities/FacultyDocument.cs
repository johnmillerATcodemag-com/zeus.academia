using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing faculty documents including CVs, portfolios, and other professional documents.
/// Supports version management, access control, and document metadata tracking.
/// </summary>
public class FacultyDocument : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic employee number this document belongs to.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the document type (CV, Portfolio, Certificate, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document title or name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document description.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the file name of the document.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original file name as uploaded.
    /// </summary>
    [MaxLength(255)]
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// Gets or sets the file path where the document is stored.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MIME type of the document.
    /// </summary>
    [MaxLength(100)]
    public string? MimeType { get; set; }

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long? FileSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the document version number.
    /// </summary>
    [Required]
    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether this is the current/latest version of the document.
    /// </summary>
    [Required]
    public bool IsCurrentVersion { get; set; } = true;

    /// <summary>
    /// Gets or sets the version notes or changelog.
    /// </summary>
    [MaxLength(500)]
    public string? VersionNotes { get; set; }

    /// <summary>
    /// Gets or sets whether the document is publicly accessible.
    /// </summary>
    [Required]
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the document is approved for publication.
    /// </summary>
    [Required]
    public bool IsApproved { get; set; } = false;

    /// <summary>
    /// Gets or sets who approved the document for publication.
    /// </summary>
    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// Gets or sets when the document was approved.
    /// </summary>
    public DateTime? ApprovalDate { get; set; }

    /// <summary>
    /// Gets or sets the document upload date.
    /// </summary>
    [Required]
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the expiration date for the document (if applicable).
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets access level required to view the document.
    /// </summary>
    [MaxLength(50)]
    public string? AccessLevel { get; set; }

    /// <summary>
    /// Gets or sets document tags for categorization and searching.
    /// </summary>
    [MaxLength(500)]
    public string? Tags { get; set; }

    /// <summary>
    /// Gets or sets document metadata in JSON format.
    /// </summary>
    [MaxLength(2000)]
    public string? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the download count for tracking popularity.
    /// </summary>
    public int DownloadCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the last download date.
    /// </summary>
    public DateTime? LastDownloadDate { get; set; }

    /// <summary>
    /// Gets or sets whether the document is archived.
    /// </summary>
    [Required]
    public bool IsArchived { get; set; } = false;

    /// <summary>
    /// Gets or sets the archive date.
    /// </summary>
    public DateTime? ArchiveDate { get; set; }

    /// <summary>
    /// Gets or sets the archive reason.
    /// </summary>
    [MaxLength(200)]
    public string? ArchiveReason { get; set; }

    /// <summary>
    /// Navigation property to the academic employee.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;

    /// <summary>
    /// Navigation property to the faculty profile.
    /// </summary>
    public virtual FacultyProfile? FacultyProfile { get; set; }
}