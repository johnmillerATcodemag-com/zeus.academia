using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a student's progress toward degree completion.
/// Part of Task 4: Academic Record Management.
/// </summary>
public class DegreeProgress : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the degree progress record.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the student's employee number.
    /// </summary>
    [Required]
    public int StudentEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the degree code the student is pursuing.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string DegreeCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total credit hours required for the degree.
    /// </summary>
    [Range(1, 300)]
    public int RequiredCreditHours { get; set; }

    /// <summary>
    /// Gets or sets the credit hours completed toward the degree.
    /// </summary>
    [Range(0, 300)]
    public decimal CompletedCreditHours { get; set; }

    /// <summary>
    /// Gets or sets the remaining credit hours needed.
    /// </summary>
    [Range(0, 300)]
    public decimal RemainingCreditHours { get; set; }

    /// <summary>
    /// Gets or sets the percentage of degree completion.
    /// </summary>
    [Range(0, 100)]
    public decimal CompletionPercentage { get; set; }

    /// <summary>
    /// Gets or sets the student's current cumulative GPA.
    /// </summary>
    [Range(0.0, 4.0)]
    public decimal CumulativeGPA { get; set; }

    /// <summary>
    /// Gets or sets the major GPA (courses in the major field).
    /// </summary>
    [Range(0.0, 4.0)]
    public decimal? MajorGPA { get; set; }

    /// <summary>
    /// Gets or sets the minimum GPA required for graduation.
    /// </summary>
    [Range(0.0, 4.0)]
    public decimal RequiredGPA { get; set; } = 2.0m;

    /// <summary>
    /// Gets or sets whether the student meets the GPA requirement.
    /// </summary>
    public bool MeetsGPARequirement { get; set; }

    /// <summary>
    /// Gets or sets the expected graduation date.
    /// </summary>
    public DateTime? ExpectedGraduationDate { get; set; }

    /// <summary>
    /// Gets or sets the projected graduation term.
    /// </summary>
    [MaxLength(20)]
    public string? ProjectedGraduationTerm { get; set; }

    /// <summary>
    /// Gets or sets the number of core requirements completed.
    /// </summary>
    public int CoreRequirementsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the total number of core requirements.
    /// </summary>
    public int TotalCoreRequirements { get; set; }

    /// <summary>
    /// Gets or sets the number of elective requirements completed.
    /// </summary>
    public int ElectiveRequirementsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the total number of elective requirements.
    /// </summary>
    public int TotalElectiveRequirements { get; set; }

    /// <summary>
    /// Gets or sets whether all prerequisite requirements are met.
    /// </summary>
    public bool PrerequisitesMet { get; set; }

    /// <summary>
    /// Gets or sets whether the capstone requirement is completed.
    /// </summary>
    public bool CapstoneCompleted { get; set; }

    /// <summary>
    /// Gets or sets whether the thesis requirement is completed (if applicable).
    /// </summary>
    public bool? ThesisCompleted { get; set; }

    /// <summary>
    /// Gets or sets whether internship requirements are completed (if applicable).
    /// </summary>
    public bool? InternshipCompleted { get; set; }

    /// <summary>
    /// Gets or sets additional requirements that must be met.
    /// </summary>
    [MaxLength(1000)]
    public string? AdditionalRequirements { get; set; }

    /// <summary>
    /// Gets or sets the date this progress record was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets who last updated this progress record.
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets notes about the student's degree progress.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    [ForeignKey(nameof(StudentEmpNr))]
    public virtual Student Student { get; set; } = null!;

    /// <summary>
    /// Navigation property to the degree.
    /// </summary>
    [ForeignKey(nameof(DegreeCode))]
    public virtual Degree Degree { get; set; } = null!;
}