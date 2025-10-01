using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a university or educational institution.
/// </summary>
public class University : BaseEntity
{
    /// <summary>
    /// Gets or sets the university code - the primary identifier.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the university name.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the university location/address.
    /// </summary>
    [MaxLength(200)]
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the university website URL.
    /// </summary>
    [MaxLength(100)]
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the accreditation status.
    /// </summary>
    [MaxLength(50)]
    public string? AccreditationStatus { get; set; }

    /// <summary>
    /// Navigation property for academics with degrees from this university.
    /// </summary>
    public virtual ICollection<AcademicDegree> AcademicDegrees { get; set; } = new List<AcademicDegree>();
}