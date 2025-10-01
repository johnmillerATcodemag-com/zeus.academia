using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic degree.
/// </summary>
public class Degree : BaseEntity
{
    /// <summary>
    /// Gets or sets the degree code - the primary identifier.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the degree title or name.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the degree level (Bachelor, Master, Doctorate, etc.).
    /// </summary>
    [MaxLength(20)]
    public string? Level { get; set; }

    /// <summary>
    /// Gets or sets the description of the degree.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Navigation property for academics with this degree.
    /// </summary>
    public virtual ICollection<AcademicDegree> AcademicDegrees { get; set; } = new List<AcademicDegree>();

    /// <summary>
    /// Navigation property for students pursuing this degree.
    /// </summary>
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
