using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

public class FacultyPublication
{
    [Key]
    public int PublicationId { get; set; }

    [Required]
    public int AcademicId { get; set; }

    [ForeignKey(nameof(AcademicId))]
    public Academic Academic { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string PublicationType { get; set; } = string.Empty; // Journal Article, Conference Paper, Book, Book Chapter, etc.

    [StringLength(200)]
    public string? Journal { get; set; }

    [StringLength(200)]
    public string? Publisher { get; set; }

    [StringLength(200)]
    public string? ConferenceName { get; set; }

    [StringLength(50)]
    public string? Volume { get; set; }

    [StringLength(50)]
    public string? Issue { get; set; }

    [StringLength(50)]
    public string? Pages { get; set; }

    [Column(TypeName = "date")]
    public DateTime? PublicationDate { get; set; }

    [Required]
    public int PublicationYear { get; set; }

    [StringLength(100)]
    public string? DOI { get; set; }

    [StringLength(20)]
    public string? ISBN { get; set; }

    [StringLength(20)]
    public string? ISSN { get; set; }

    [StringLength(2000)]
    public string? Abstract { get; set; }

    [StringLength(1000)]
    public string? Keywords { get; set; }

    [StringLength(2000)]
    public string? CoAuthors { get; set; }

    public bool IsPeerReviewed { get; set; }

    public bool IsOpenAccess { get; set; }

    [StringLength(500)]
    public string? ExternalUrl { get; set; }

    [StringLength(500)]
    public string? FilePath { get; set; }

    public int CitationCount { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } // Published, In Press, Under Review, Draft

    [StringLength(200)]
    public string? ResearchArea { get; set; }

    [StringLength(100)]
    public string? FundingSource { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    // Audit fields
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string LastModifiedBy { get; set; } = string.Empty;

    // Navigation properties
    public FacultyProfile? FacultyProfile { get; set; }
}