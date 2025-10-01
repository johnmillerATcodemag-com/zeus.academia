using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic subject or course.
/// </summary>
public class Subject : BaseEntity
{
    /// <summary>
    /// Gets or sets the subject code - the primary identifier.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject title or name.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the number of credit hours for this subject.
    /// </summary>
    public int? CreditHours { get; set; }

    /// <summary>
    /// Gets or sets the department that offers this subject.
    /// </summary>
    [MaxLength(15)]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the subject level (undergraduate, graduate, etc.).
    /// </summary>
    [MaxLength(20)]
    public string? Level { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Navigation property for teaching records of this subject.
    /// </summary>
    public virtual ICollection<Teaching> Teachings { get; set; } = new List<Teaching>();
}

/// <summary>
/// Entity representing the teaching relationship between an academic and a subject.
/// </summary>
public class Teaching : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the teaching record.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the academic's employee number.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the subject code.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string SubjectCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the semester when teaching occurred.
    /// </summary>
    [MaxLength(20)]
    public string? Semester { get; set; }

    /// <summary>
    /// Gets or sets the academic year.
    /// </summary>
    public int? AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the number of students enrolled.
    /// </summary>
    public int? EnrollmentCount { get; set; }

    /// <summary>
    /// Navigation property to the academic.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;

    /// <summary>
    /// Navigation property to the subject.
    /// </summary>
    public virtual Subject Subject { get; set; } = null!;
}