using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a Professor in the academic system.
/// Inherits from Academic and adds professor-specific properties.
/// </summary>
public class Professor : Academic
{
    /// <summary>
    /// Gets or sets the academic rank of the professor.
    /// </summary>
    [MaxLength(10)]
    public string? RankCode { get; set; }

    /// <summary>
    /// Gets or sets the department code where the professor works.
    /// </summary>
    [MaxLength(15)]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets whether the professor has tenure.
    /// </summary>
    public bool? HasTenure { get; set; }

    /// <summary>
    /// Gets or sets the professor's research area or specialty.
    /// </summary>
    [MaxLength(100)]
    public string? ResearchArea { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Navigation property to the rank.
    /// </summary>
    public virtual Rank? Rank { get; set; }

    /// <summary>
    /// Navigation property for committees where this professor serves.
    /// </summary>
    public virtual ICollection<CommitteeMember> CommitteeMembers { get; set; } = new List<CommitteeMember>();

    /// <summary>
    /// Navigation property for subjects this professor teaches.
    /// </summary>
    public virtual ICollection<Teaching> Teachings { get; set; } = new List<Teaching>();
}