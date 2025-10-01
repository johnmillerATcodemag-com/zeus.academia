using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic rank.
/// </summary>
public class Rank : BaseEntity
{
    /// <summary>
    /// Gets or sets the rank code - the primary identifier.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rank title or name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rank level or hierarchy order.
    /// </summary>
    public int? Level { get; set; }

    /// <summary>
    /// Gets or sets the description of the rank.
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the minimum salary for this rank.
    /// </summary>
    public decimal? MinSalary { get; set; }

    /// <summary>
    /// Gets or sets the maximum salary for this rank.
    /// </summary>
    public decimal? MaxSalary { get; set; }

    /// <summary>
    /// Navigation property for professors with this rank.
    /// </summary>
    public virtual ICollection<Professor> Professors { get; set; } = new List<Professor>();

    /// <summary>
    /// Navigation property for teaching professors with this rank.
    /// </summary>
    public virtual ICollection<TeachingProf> TeachingProfs { get; set; } = new List<TeachingProf>();
}