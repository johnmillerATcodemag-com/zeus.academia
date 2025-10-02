using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a grade record for a student's course enrollment.
/// Part of Task 4: Academic Record Management.
/// </summary>
public class Grade : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the grade.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the course enrollment ID this grade belongs to.
    /// </summary>
    [Required]
    public int CourseEnrollmentId { get; set; }

    /// <summary>
    /// Gets or sets the type of grade (midterm, final, assignment, etc.).
    /// </summary>
    public GradeType GradeType { get; set; }

    /// <summary>
    /// Gets or sets the letter grade (A, B, C, D, F, etc.).
    /// </summary>
    [MaxLength(5)]
    public string? LetterGrade { get; set; }

    /// <summary>
    /// Gets or sets the numeric grade (0-100).
    /// </summary>
    [Range(0, 100)]
    public decimal? NumericGrade { get; set; }

    /// <summary>
    /// Gets or sets the grade points for GPA calculation.
    /// </summary>
    [Range(0, 4.0)]
    public decimal? GradePoints { get; set; }

    /// <summary>
    /// Gets or sets the credit hours this grade represents.
    /// </summary>
    [Range(0.5, 12.0)]
    public decimal CreditHours { get; set; }

    /// <summary>
    /// Gets or sets the quality points (grade points * credit hours).
    /// </summary>
    public decimal QualityPoints { get; set; }

    /// <summary>
    /// Gets or sets the grade status.
    /// </summary>
    public GradeStatus Status { get; set; } = GradeStatus.Posted;

    /// <summary>
    /// Gets or sets whether this grade is final.
    /// </summary>
    public bool IsFinal { get; set; } = false;

    /// <summary>
    /// Gets or sets the date the grade was recorded.
    /// </summary>
    public DateTime GradeDate { get; set; }

    /// <summary>
    /// Gets or sets the date the grade was posted/finalized.
    /// </summary>
    public DateTime? PostedDate { get; set; }

    /// <summary>
    /// Gets or sets the instructor who assigned the grade.
    /// </summary>
    [MaxLength(100)]
    public string? GradedBy { get; set; }

    /// <summary>
    /// Gets or sets additional comments about the grade.
    /// </summary>
    [MaxLength(500)]
    public string? Comments { get; set; }

    /// <summary>
    /// Gets or sets whether this grade was earned through makeup work.
    /// </summary>
    public bool IsMakeup { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this grade replaces a previous grade.
    /// </summary>
    public bool IsReplacement { get; set; } = false;

    /// <summary>
    /// Gets or sets the ID of the grade this replaces (if applicable).
    /// </summary>
    public int? ReplacedGradeId { get; set; }

    /// <summary>
    /// Navigation property to the course enrollment.
    /// </summary>
    [ForeignKey(nameof(CourseEnrollmentId))]
    public virtual CourseEnrollment CourseEnrollment { get; set; } = null!;

    /// <summary>
    /// Navigation property to the replaced grade (if applicable).
    /// </summary>
    [ForeignKey(nameof(ReplacedGradeId))]
    public virtual Grade? ReplacedGrade { get; set; }
}