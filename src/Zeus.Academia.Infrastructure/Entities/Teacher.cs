using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a Teacher in the academic system.
/// Inherits from Academic and adds teacher-specific properties.
/// </summary>
public class Teacher : Academic
{
    /// <summary>
    /// Gets or sets the department where the teacher works.
    /// </summary>
    [MaxLength(15)]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the teacher's specialization or subject area.
    /// </summary>
    [MaxLength(100)]
    public string? Specialization { get; set; }

    /// <summary>
    /// Gets or sets the employment type (full-time, part-time, adjunct, etc.).
    /// </summary>
    [MaxLength(20)]
    public string? EmploymentType { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of courses the teacher can teach per semester.
    /// </summary>
    public int? MaxCourseLoad { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Navigation property for subjects this teacher teaches.
    /// </summary>
    public virtual ICollection<Teaching> Teachings { get; set; } = new List<Teaching>();

    /// <summary>
    /// Navigation property for ratings given to the teacher.
    /// </summary>
    public virtual ICollection<TeacherRating> TeacherRatings { get; set; } = new List<TeacherRating>();
}