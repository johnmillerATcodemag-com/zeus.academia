using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic honor awarded to a student.
/// Part of Task 4: Academic Record Management.
/// </summary>
public class AcademicHonor : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the academic honor.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the student's employee number.
    /// </summary>
    [Required]
    public int StudentEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the type of honor.
    /// </summary>
    public HonorType HonorType { get; set; }

    /// <summary>
    /// Gets or sets the title of the honor.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the honor.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the academic term when the honor was awarded.
    /// </summary>
    public int? AcademicTermId { get; set; }

    /// <summary>
    /// Gets or sets the semester when the honor was awarded.
    /// </summary>
    [MaxLength(20)]
    public string? Semester { get; set; }

    /// <summary>
    /// Gets or sets the academic year when the honor was awarded.
    /// </summary>
    public int AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the date the honor was awarded.
    /// </summary>
    public DateTime AwardDate { get; set; }

    /// <summary>
    /// Gets or sets the GPA requirement that was met (if applicable).
    /// </summary>
    [Range(0.0, 4.0)]
    public decimal? RequiredGPA { get; set; }

    /// <summary>
    /// Gets or sets the student's GPA when the honor was awarded.
    /// </summary>
    [Range(0.0, 4.0)]
    public decimal? StudentGPA { get; set; }

    /// <summary>
    /// Gets or sets the minimum credit hours required (if applicable).
    /// </summary>
    public int? MinimumCreditHours { get; set; }

    /// <summary>
    /// Gets or sets whether this honor appears on transcripts.
    /// </summary>
    public bool AppearsOnTranscript { get; set; } = true;

    /// <summary>
    /// Gets or sets the organization that awarded the honor.
    /// </summary>
    [MaxLength(100)]
    public string? AwardingOrganization { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the honor.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether the honor is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    [ForeignKey(nameof(StudentEmpNr))]
    public virtual Student Student { get; set; } = null!;

    /// <summary>
    /// Navigation property to the academic term.
    /// </summary>
    [ForeignKey(nameof(AcademicTermId))]
    public virtual AcademicTerm? AcademicTerm { get; set; }
}