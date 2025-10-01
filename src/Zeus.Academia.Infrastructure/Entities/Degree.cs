using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic degree.
/// Enhanced for Task 3: Academic Structure Entities with comprehensive validation and degree management features.
/// </summary>
public class Degree : BaseEntity
{
    /// <summary>
    /// Gets or sets the degree code - the primary identifier.
    /// </summary>
    [Key]
    [Required(ErrorMessage = "Degree code is required")]
    [MaxLength(10, ErrorMessage = "Degree code cannot exceed 10 characters")]
    [RegularExpression(@"^[A-Z]{2,4}[0-9]{0,2}$", ErrorMessage = "Degree code must be 2-4 uppercase letters optionally followed by up to 2 numbers (e.g., BS, MS, PHD, BSCS, MBA1)")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the degree title or name.
    /// </summary>
    [Required(ErrorMessage = "Degree title is required")]
    [MaxLength(100, ErrorMessage = "Degree title cannot exceed 100 characters")]
    [MinLength(10, ErrorMessage = "Degree title must be at least 10 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the degree level (Bachelor, Master, Doctorate, etc.).
    /// </summary>
    [Required(ErrorMessage = "Degree level is required")]
    [MaxLength(20, ErrorMessage = "Degree level cannot exceed 20 characters")]
    [RegularExpression(@"^(Associate|Bachelor|Master|Doctorate|Certificate|Diploma)$", ErrorMessage = "Degree level must be one of: Associate, Bachelor, Master, Doctorate, Certificate, Diploma")]
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the degree.
    /// </summary>
    [MaxLength(500, ErrorMessage = "Degree description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the total credit hours required for this degree.
    /// </summary>
    [Range(30, 300, ErrorMessage = "Total credit hours must be between 30 and 300")]
    public int? TotalCreditHours { get; set; }

    /// <summary>
    /// Gets or sets the typical duration of the degree program in years.
    /// </summary>
    [Range(1, 10, ErrorMessage = "Duration must be between 1 and 10 years")]
    [Column(TypeName = "decimal(3,1)")]
    public decimal? DurationYears { get; set; }

    /// <summary>
    /// Gets or sets whether the degree is currently active and being offered.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the department that primarily offers this degree.
    /// </summary>
    [MaxLength(15, ErrorMessage = "Department name cannot exceed 15 characters")]
    public string? PrimaryDepartment { get; set; }

    /// <summary>
    /// Gets or sets the degree specialization or concentration area.
    /// </summary>
    [MaxLength(100, ErrorMessage = "Specialization cannot exceed 100 characters")]
    public string? Specialization { get; set; }

    /// <summary>
    /// Gets or sets the minimum GPA required for graduation.
    /// </summary>
    [Range(1.0, 4.0, ErrorMessage = "Minimum GPA must be between 1.0 and 4.0")]
    [Column(TypeName = "decimal(3,2)")]
    public decimal? MinimumGPA { get; set; }

    /// <summary>
    /// Navigation property for academics with this degree.
    /// </summary>
    public virtual ICollection<AcademicDegree> AcademicDegrees { get; set; } = new List<AcademicDegree>();

    /// <summary>
    /// Navigation property for students pursuing this degree.
    /// </summary>
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
