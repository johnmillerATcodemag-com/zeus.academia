using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing documents submitted with an enrollment application
/// </summary>
public class ApplicationDocument : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the document.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the application ID this document belongs to.
    /// </summary>
    [Required]
    public int ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the document type (transcript, recommendation letter, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original file name.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file path or storage location.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the file.
    /// </summary>
    [MaxLength(100)]
    public string? MimeType { get; set; }

    /// <summary>
    /// Gets or sets the upload date.
    /// </summary>
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets whether this document is required for the application.
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this document has been verified.
    /// </summary>
    public bool IsVerified { get; set; } = false;

    /// <summary>
    /// Gets or sets the date when the document was verified.
    /// </summary>
    public DateTime? VerificationDate { get; set; }

    /// <summary>
    /// Gets or sets who verified the document.
    /// </summary>
    [MaxLength(100)]
    public string? VerifiedBy { get; set; }

    /// <summary>
    /// Gets or sets notes about the document.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the enrollment application.
    /// </summary>
    public virtual EnrollmentApplication Application { get; set; } = null!;
}