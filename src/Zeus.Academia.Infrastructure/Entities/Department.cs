using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic department.
/// </summary>
public class Department : BaseEntity
{
    /// <summary>
    /// Gets or sets the department name - the primary identifier.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(15)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name or title of the department.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the department description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the department head's employee number.
    /// </summary>
    public int? HeadEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the department's budget.
    /// </summary>
    public decimal? Budget { get; set; }

    /// <summary>
    /// Navigation property to the department head.
    /// </summary>
    public virtual Academic? Head { get; set; }

    /// <summary>
    /// Navigation property for professors in this department.
    /// </summary>
    public virtual ICollection<Professor> Professors { get; set; } = new List<Professor>();

    /// <summary>
    /// Navigation property for teachers in this department.
    /// </summary>
    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    /// <summary>
    /// Navigation property for teaching professors in this department.
    /// </summary>
    public virtual ICollection<TeachingProf> TeachingProfs { get; set; } = new List<TeachingProf>();

    /// <summary>
    /// Navigation property for students in this department.
    /// </summary>
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    /// <summary>
    /// Navigation property for chairs in this department.
    /// </summary>
    public virtual ICollection<Chair> Chairs { get; set; } = new List<Chair>();

    /// <summary>
    /// Navigation property for subjects offered by this department.
    /// </summary>
    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}