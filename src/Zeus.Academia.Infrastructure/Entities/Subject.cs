using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic subject or course.
/// Enhanced for Task 3: Academic Structure Entities with comprehensive validation and relationships.
/// </summary>
public class Subject : BaseEntity
{
    /// <summary>
    /// Gets or sets the subject code - the primary identifier.
    /// </summary>
    [Key]
    [Required(ErrorMessage = "Subject code is required")]
    [MaxLength(10, ErrorMessage = "Subject code cannot exceed 10 characters")]
    [RegularExpression(@"^[A-Z]{2,4}[0-9]{2,4}[A-Z]?$", ErrorMessage = "Subject code must follow format: 2-4 letters followed by 2-4 numbers, optionally ending with a letter (e.g., CS101, MATH2010A)")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject title or name.
    /// </summary>
    [Required(ErrorMessage = "Subject title is required")]
    [MaxLength(100, ErrorMessage = "Subject title cannot exceed 100 characters")]
    [MinLength(5, ErrorMessage = "Subject title must be at least 5 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject description.
    /// </summary>
    [MaxLength(500, ErrorMessage = "Subject description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the number of credit hours for this subject.
    /// </summary>
    [Range(1, 12, ErrorMessage = "Credit hours must be between 1 and 12")]
    public int? CreditHours { get; set; }

    /// <summary>
    /// Gets or sets the department that offers this subject.
    /// </summary>
    [MaxLength(15, ErrorMessage = "Department name cannot exceed 15 characters")]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the subject level (undergraduate, graduate, etc.).
    /// </summary>
    [MaxLength(20, ErrorMessage = "Subject level cannot exceed 20 characters")]
    [RegularExpression(@"^(Undergraduate|Graduate|Doctoral|Continuing Education)$", ErrorMessage = "Subject level must be one of: Undergraduate, Graduate, Doctoral, Continuing Education")]
    public string? Level { get; set; }

    /// <summary>
    /// Gets or sets the subject prerequisites as a comma-separated list of subject codes.
    /// </summary>
    [MaxLength(200, ErrorMessage = "Prerequisites cannot exceed 200 characters")]
    public string? Prerequisites { get; set; }

    /// <summary>
    /// Gets or sets whether the subject is currently active and being offered.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the semester when the subject is typically offered.
    /// </summary>
    [MaxLength(20)]
    [RegularExpression(@"^(Fall|Spring|Summer|Year-round)$", ErrorMessage = "Semester must be one of: Fall, Spring, Summer, Year-round")]
    public string? TypicalSemester { get; set; }

    /// <summary>
    /// Gets or sets the maximum enrollment capacity for this subject.
    /// </summary>
    [Range(1, 500, ErrorMessage = "Maximum enrollment must be between 1 and 500")]
    public int? MaxEnrollment { get; set; }

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