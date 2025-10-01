using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic department.
/// Enhanced for Task 3: Academic Structure Entities with comprehensive validation and relationships.
/// </summary>
public class Department : BaseEntity
{
    /// <summary>
    /// Gets or sets the department name - the primary identifier.
    /// </summary>
    [Key]
    [Required(ErrorMessage = "Department name is required")]
    [MaxLength(15, ErrorMessage = "Department name cannot exceed 15 characters")]
    [RegularExpression(@"^[A-Z][A-Za-z\s]*$", ErrorMessage = "Department name must start with uppercase letter and contain only letters and spaces")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name or title of the department.
    /// </summary>
    [Required(ErrorMessage = "Department full name is required")]
    [MaxLength(100, ErrorMessage = "Department full name cannot exceed 100 characters")]
    [MinLength(5, ErrorMessage = "Department full name must be at least 5 characters")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the department description.
    /// </summary>
    [MaxLength(500, ErrorMessage = "Department description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the department head's employee number.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Department head employee number must be positive")]
    public int? HeadEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the department's budget.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Department budget must be non-negative")]
    [Column(TypeName = "decimal(15,2)")]
    public decimal? Budget { get; set; }

    /// <summary>
    /// Gets or sets the department's establishment date.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? EstablishedDate { get; set; }

    /// <summary>
    /// Gets or sets whether the department is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the department's physical location.
    /// </summary>
    [MaxLength(100, ErrorMessage = "Department location cannot exceed 100 characters")]
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the department's contact phone number.
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the department's email address.
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [MaxLength(100)]
    public string? Email { get; set; }

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