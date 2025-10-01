using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a rating given to a teacher.
/// </summary>
public class TeacherRating : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the rating.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the academic's employee number who received the rating.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the rating value.
    /// </summary>
    [Required]
    public int RatingValue { get; set; }

    /// <summary>
    /// Gets or sets the subject code for which the rating was given.
    /// </summary>
    [MaxLength(10)]
    public string? SubjectCode { get; set; }

    /// <summary>
    /// Gets or sets the semester when the rating was given.
    /// </summary>
    [MaxLength(20)]
    public string? Semester { get; set; }

    /// <summary>
    /// Gets or sets the academic year.
    /// </summary>
    public int? AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets additional comments about the rating.
    /// </summary>
    [MaxLength(1000)]
    public string? Comments { get; set; }

    /// <summary>
    /// Gets or sets the source of the rating (student, peer, administration, etc.).
    /// </summary>
    [MaxLength(30)]
    public string? RatingSource { get; set; }

    /// <summary>
    /// Navigation property to the academic who received the rating.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;

    /// <summary>
    /// Navigation property to the subject.
    /// </summary>
    public virtual Subject? Subject { get; set; }
}

/// <summary>
/// Entity representing student enrollment in courses/subjects.
/// </summary>
public class StudentEnrollment : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the enrollment.
    /// </summary>
    [Key]
    public int Id { get; set; }

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
    /// Gets or sets the semester of enrollment.
    /// </summary>
    [MaxLength(20)]
    public string? Semester { get; set; }

    /// <summary>
    /// Gets or sets the academic year.
    /// </summary>
    public int? AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the grade received (if completed).
    /// </summary>
    [MaxLength(5)]
    public string? Grade { get; set; }

    /// <summary>
    /// Gets or sets the enrollment status (enrolled, completed, dropped, etc.).
    /// </summary>
    [MaxLength(20)]
    public string? Status { get; set; } = "Enrolled";

    /// <summary>
    /// Gets or sets the enrollment date.
    /// </summary>
    public DateTime? EnrollmentDate { get; set; }

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    public virtual Student Student { get; set; } = null!;

    /// <summary>
    /// Navigation property to the subject.
    /// </summary>
    public virtual Subject Subject { get; set; } = null!;
}