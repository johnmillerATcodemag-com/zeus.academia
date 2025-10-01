using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Base entity for all academic personnel in the system.
/// Serves as the parent class for Professor, Teacher, Chair, TeachingProf, and Student.
/// </summary>
public abstract class Academic : BaseEntity
{
    /// <summary>
    /// Gets or sets the employee number - the primary identifier for academic personnel.
    /// </summary>
    [Key]
    [Required]
    public int EmpNr { get; set; }

    /// <summary>
    /// Gets or sets the name of the academic person.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number of the academic person.
    /// </summary>
    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the salary of the academic person.
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// Navigation property for relationships and derived types.
    /// </summary>
    public virtual ICollection<AcademicDegree> AcademicDegrees { get; set; } = new List<AcademicDegree>();
}

/// <summary>
/// Entity representing the relationship between Academic and their degrees from universities.
/// </summary>
public class AcademicDegree : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic's employee number.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the degree code.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string DegreeCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the university code.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string UniversityCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the degree was obtained.
    /// </summary>
    public DateTime? DateObtained { get; set; }

    /// <summary>
    /// Navigation property to the Academic.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;

    /// <summary>
    /// Navigation property to the Degree.
    /// </summary>
    public virtual Degree Degree { get; set; } = null!;

    /// <summary>
    /// Navigation property to the University.
    /// </summary>
    public virtual University University { get; set; } = null!;
}