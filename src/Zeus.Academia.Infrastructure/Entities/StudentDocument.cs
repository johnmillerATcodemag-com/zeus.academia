using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a document or photo uploaded for a student
/// </summary>
public class StudentDocument : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the document.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the student's employee number this document belongs to.
    /// </summary>
    public int StudentEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the document type.
    /// </summary>
    public StudentDocumentType DocumentType { get; set; }

    /// <summary>
    /// Gets or sets the original filename.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stored filename (may be different from original for security).
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string StoredFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file path relative to the storage root.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MIME type of the file.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the file hash for integrity verification.
    /// </summary>
    [MaxLength(128)]
    public string? FileHash { get; set; }

    /// <summary>
    /// Gets or sets a description or caption for the document.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets who uploaded the document.
    /// </summary>
    [MaxLength(100)]
    public string? UploadedBy { get; set; }

    /// <summary>
    /// Gets or sets when the document was uploaded.
    /// </summary>
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets whether the document is currently active/visible.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this document is public or private.
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the document has been verified by staff.
    /// </summary>
    public bool IsVerified { get; set; } = false;

    /// <summary>
    /// Gets or sets who verified the document.
    /// </summary>
    [MaxLength(100)]
    public string? VerifiedBy { get; set; }

    /// <summary>
    /// Gets or sets when the document was verified.
    /// </summary>
    public DateTime? VerificationDate { get; set; }

    /// <summary>
    /// Gets or sets the expiration date for the document (if applicable).
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets whether the document is required for profile completion.
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Gets or sets access level for the document.
    /// </summary>
    public DocumentAccessLevel AccessLevel { get; set; } = DocumentAccessLevel.StudentOnly;

    /// <summary>
    /// Gets or sets any notes about this document.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets tags for categorizing or searching documents.
    /// </summary>
    [MaxLength(500)]
    public string? Tags { get; set; }

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    public virtual Student? Student { get; set; }
}