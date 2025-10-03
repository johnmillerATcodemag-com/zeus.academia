using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing research areas for categorizing faculty expertise.
/// Provides a standardized taxonomy of research domains.
/// </summary>
public class ResearchArea : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique code for the research area.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the research area.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the research area.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the parent research area code (for hierarchical organization).
    /// </summary>
    [MaxLength(20)]
    public string? ParentAreaCode { get; set; }

    /// <summary>
    /// Gets or sets the discipline or field this research area belongs to.
    /// </summary>
    [MaxLength(100)]
    public string? PrimaryDiscipline { get; set; }

    /// <summary>
    /// Gets or sets whether this research area is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the display order for listing research areas.
    /// </summary>
    public int? DisplayOrder { get; set; }

    /// <summary>
    /// Navigation property to parent research area.
    /// </summary>
    public virtual ResearchArea? ParentArea { get; set; }

    /// <summary>
    /// Navigation property to child research areas.
    /// </summary>
    public virtual ICollection<ResearchArea> ChildAreas { get; set; } = new List<ResearchArea>();

    /// <summary>
    /// Navigation property to faculty expertise records.
    /// </summary>
    public virtual ICollection<FacultyExpertise> FacultyExpertise { get; set; } = new List<FacultyExpertise>();
}

/// <summary>
/// Entity representing faculty expertise in specific research areas.
/// Many-to-many relationship between faculty and research areas with additional metadata.
/// </summary>
public class FacultyExpertise : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic employee number.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the research area code.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string ResearchAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the level of expertise (Beginner, Intermediate, Advanced, Expert).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ExpertiseLevel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this is a primary area of expertise.
    /// </summary>
    [Required]
    public bool IsPrimaryExpertise { get; set; } = false;

    /// <summary>
    /// Gets or sets the years of experience in this research area.
    /// </summary>
    public int? YearsOfExperience { get; set; }

    /// <summary>
    /// Gets or sets the date when this expertise was first established.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the faculty member's expertise in this area.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets relevant certifications or credentials in this area.
    /// </summary>
    [MaxLength(500)]
    public string? Certifications { get; set; }

    /// <summary>
    /// Gets or sets the number of publications in this research area.
    /// </summary>
    public int? PublicationCount { get; set; }

    /// <summary>
    /// Gets or sets the number of grants received in this research area.
    /// </summary>
    public int? GrantCount { get; set; }

    /// <summary>
    /// Navigation property to the academic employee.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;

    /// <summary>
    /// Navigation property to the research area.
    /// </summary>
    public virtual ResearchArea ResearchArea { get; set; } = null!;
}