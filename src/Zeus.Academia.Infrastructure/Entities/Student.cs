using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Enums;

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
    /// Gets or sets the student's current enrollment status.
    /// </summary>
    public EnrollmentStatus EnrollmentStatus { get; set; } = EnrollmentStatus.Applied;

    /// <summary>
    /// Gets or sets the student's academic standing.
    /// </summary>
    public AcademicStanding AcademicStanding { get; set; } = AcademicStanding.NewStudent;

    /// <summary>
    /// Gets or sets the current cumulative GPA for the student.
    /// </summary>
    public decimal? CumulativeGPA { get; set; }

    /// <summary>
    /// Gets or sets the current semester GPA for the student.
    /// </summary>
    public decimal? SemesterGPA { get; set; }

    /// <summary>
    /// Gets or sets the total credit hours attempted by the student.
    /// </summary>
    public int? TotalCreditHoursAttempted { get; set; }

    /// <summary>
    /// Gets or sets the total credit hours earned by the student.
    /// </summary>
    public int? TotalCreditHoursEarned { get; set; }

    /// <summary>
    /// Gets or sets the credit hours needed for graduation.
    /// </summary>
    public int? CreditHoursRequired { get; set; }

    /// <summary>
    /// Gets or sets the last academic standing review date.
    /// </summary>
    public DateTime? LastAcademicReviewDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the student's enrollment status was last updated.
    /// </summary>
    public DateTime? EnrollmentStatusDate { get; set; }

    /// <summary>
    /// Gets or sets the academic advisor assigned to the student.
    /// </summary>
    [MaxLength(50)]
    public string? AcademicAdvisor { get; set; }

    /// <summary>
    /// Gets or sets the current academic term/semester.
    /// </summary>
    [MaxLength(20)]
    public string? CurrentTerm { get; set; }

    /// <summary>
    /// Gets or sets admission date.
    /// </summary>
    public DateTime? AdmissionDate { get; set; }

    /// <summary>
    /// Gets or sets the graduation date (if graduated).
    /// </summary>
    public DateTime? ActualGraduationDate { get; set; }

    /// <summary>
    /// Gets or sets notes about the student's academic progress or status.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether the student is a full-time or part-time student.
    /// </summary>
    public bool IsFullTime { get; set; } = true;

    /// <summary>
    /// Navigation property to the degree.
    /// </summary>
    public virtual Degree? Degree { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Gets the department ID from the associated department.
    /// </summary>
    public override int? DepartmentId => Department?.Id;

    /// <summary>
    /// Navigation property for courses the student is enrolled in.
    /// </summary>
    public virtual ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();
}