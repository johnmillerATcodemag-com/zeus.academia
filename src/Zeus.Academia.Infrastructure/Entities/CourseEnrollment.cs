using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a student's enrollment in a specific course section.
/// Enhanced version of StudentEnrollment for Task 4: Academic Record Management.
/// </summary>
public class CourseEnrollment : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the enrollment.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the student's employee number.
    /// </summary>
    [Required]
    public int StudentEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the subject code.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string SubjectCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the section identifier for the course.
    /// </summary>
    [MaxLength(10)]
    public string? SectionId { get; set; }

    /// <summary>
    /// Gets or sets the academic term ID.
    /// </summary>
    public int? AcademicTermId { get; set; }

    /// <summary>
    /// Gets or sets the semester of enrollment.
    /// </summary>
    [MaxLength(20)]
    public string? Semester { get; set; }

    /// <summary>
    /// Gets or sets the academic year.
    /// </summary>
    public int AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the enrollment status.
    /// </summary>
    public CourseEnrollmentStatus Status { get; set; } = CourseEnrollmentStatus.Enrolled;

    /// <summary>
    /// Gets or sets the enrollment date.
    /// </summary>
    public DateTime EnrollmentDate { get; set; }

    /// <summary>
    /// Gets or sets the drop date (if applicable).
    /// </summary>
    public DateTime? DropDate { get; set; }

    /// <summary>
    /// Gets or sets the withdrawal date (if applicable).
    /// </summary>
    public DateTime? WithdrawalDate { get; set; }

    /// <summary>
    /// Gets or sets the credit hours for this enrollment.
    /// </summary>
    [Range(0.5, 12.0)]
    public decimal CreditHours { get; set; }

    /// <summary>
    /// Gets or sets whether the student is auditing the course.
    /// </summary>
    public bool IsAudit { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this enrollment counts toward degree requirements.
    /// </summary>
    public bool CountsTowardDegree { get; set; } = true;

    /// <summary>
    /// Gets or sets additional enrollment notes.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    [ForeignKey(nameof(StudentEmpNr))]
    public virtual Student Student { get; set; } = null!;

    /// <summary>
    /// Navigation property to the subject.
    /// </summary>
    [ForeignKey(nameof(SubjectCode))]
    public virtual Subject Subject { get; set; } = null!;

    /// <summary>
    /// Navigation property to the academic term.
    /// </summary>
    [ForeignKey(nameof(AcademicTermId))]
    public virtual AcademicTerm? AcademicTerm { get; set; }

    /// <summary>
    /// Navigation property to grades for this enrollment.
    /// </summary>
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}