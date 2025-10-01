using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a Student in the academic system.
/// Inherits from Academic and adds student-specific properties.
/// Note: Student was not found in the ORM model but is included for completeness.
/// </summary>
public class Student : Academic
{
    /// <summary>
    /// Gets or sets the student's unique identifier number.
    /// </summary>
    [MaxLength(20)]
    public string? StudentId { get; set; }

    /// <summary>
    /// Gets or sets the program or major the student is enrolled in.
    /// </summary>
    [MaxLength(50)]
    public string? Program { get; set; }

    /// <summary>
    /// Gets or sets the degree the student is pursuing.
    /// </summary>
    [MaxLength(10)]
    public string? DegreeCode { get; set; }

    /// <summary>
    /// Gets or sets the department where the student is enrolled.
    /// </summary>
    [MaxLength(15)]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the student's year of study (1, 2, 3, 4, etc.).
    /// </summary>
    public int? YearOfStudy { get; set; }

    /// <summary>
    /// Gets or sets the student's current GPA.
    /// </summary>
    public decimal? GPA { get; set; }

    /// <summary>
    /// Gets or sets the date when the student enrolled.
    /// </summary>
    public DateTime? EnrollmentDate { get; set; }

    /// <summary>
    /// Gets or sets the expected graduation date.
    /// </summary>
    public DateTime? ExpectedGraduationDate { get; set; }

    /// <summary>
    /// Gets or sets whether the student is currently active/enrolled.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to the degree.
    /// </summary>
    public virtual Degree? Degree { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Navigation property for courses the student is enrolled in.
    /// </summary>
    public virtual ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();
}