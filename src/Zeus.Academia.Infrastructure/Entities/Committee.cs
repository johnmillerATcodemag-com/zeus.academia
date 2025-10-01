using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a committee in the academic system.
/// </summary>
public class Committee : BaseEntity
{
    /// <summary>
    /// Gets or sets the committee name - the primary identifier.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the committee description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the committee type (academic, administrative, etc.).
    /// </summary>
    [MaxLength(30)]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets whether the committee is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property for committee members.
    /// </summary>
    public virtual ICollection<CommitteeMember> Members { get; set; } = new List<CommitteeMember>();
}

/// <summary>
/// Entity representing membership of an academic in a committee.
/// </summary>
public class CommitteeMember : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the committee membership.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the committee name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string CommitteeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the academic's employee number.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the role of the academic in the committee.
    /// </summary>
    [MaxLength(30)]
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the start date of membership.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of membership.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets whether the membership is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to the committee.
    /// </summary>
    public virtual Committee Committee { get; set; } = null!;

    /// <summary>
    /// Navigation property to the academic.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;
}