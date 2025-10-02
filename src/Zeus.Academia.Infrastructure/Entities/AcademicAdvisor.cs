using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic advisor and their student assignments
/// </summary>
public class AcademicAdvisor : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the academic advisor.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the advisor's employee number (links to Faculty).
    /// </summary>
    public int? FacultyEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the advisor's full name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string AdvisorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the advisor's email address.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the advisor's phone number.
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the advisor's office location.
    /// </summary>
    [MaxLength(100)]
    public string? OfficeLocation { get; set; }

    /// <summary>
    /// Gets or sets the department the advisor belongs to.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the advisor's academic title.
    /// </summary>
    [MaxLength(100)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the advisor's areas of expertise.
    /// </summary>
    [MaxLength(500)]
    public string? Specializations { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of students this advisor can handle.
    /// </summary>
    public int? MaxStudentLoad { get; set; }

    /// <summary>
    /// Gets or sets the current number of students assigned to this advisor.
    /// </summary>
    public int CurrentStudentCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether the advisor is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the advisor is currently accepting new students.
    /// </summary>
    public bool IsAcceptingNewStudents { get; set; } = true;

    /// <summary>
    /// Gets or sets the advisor's preferred contact method.
    /// </summary>
    public ContactMethod PreferredContactMethod { get; set; } = ContactMethod.Email;

    /// <summary>
    /// Gets or sets the advisor's office hours.
    /// </summary>
    [MaxLength(500)]
    public string? OfficeHours { get; set; }

    /// <summary>
    /// Gets or sets any special notes about this advisor.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the date this advisor started.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date this advisor ended (if inactive).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Navigation property to the academic (faculty member) if applicable.
    /// Note: FacultyEmpNr references Academic.EmpNr for Professor, Teacher, etc.
    /// </summary>
    public virtual Academic? FacultyMember { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Navigation property to the students assigned to this advisor.
    /// </summary>
    public virtual ICollection<StudentAdvisorAssignment> StudentAssignments { get; set; } = new List<StudentAdvisorAssignment>();
}