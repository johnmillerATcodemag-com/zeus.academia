using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing faculty employment history and contract management.
/// Tracks employment changes, contract renewals, and position modifications over time.
/// </summary>
public class FacultyEmploymentHistory : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic employee number this employment record belongs to.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the position title (Professor, Associate Professor, Assistant Professor, etc.).
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string PositionTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the department where the faculty member was employed.
    /// </summary>
    [MaxLength(15)]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the employment type (Full-time, Part-time, Adjunct, Visiting, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string EmploymentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the contract type (Tenure-track, Tenured, Term, Lecturer, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ContractType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of this employment period.
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of this employment period (null if current).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this is the current/active employment record.
    /// </summary>
    [Required]
    public bool IsCurrentPosition { get; set; } = false;

    /// <summary>
    /// Gets or sets the annual salary for this employment period.
    /// </summary>
    public decimal? AnnualSalary { get; set; }

    /// <summary>
    /// Gets or sets the FTE (Full-Time Equivalent) percentage.
    /// </summary>
    public decimal? FtePercentage { get; set; } = 100m;

    /// <summary>
    /// Gets or sets the teaching load percentage.
    /// </summary>
    public decimal? TeachingLoadPercentage { get; set; }

    /// <summary>
    /// Gets or sets the research expectation percentage.
    /// </summary>
    public decimal? ResearchExpectationPercentage { get; set; }

    /// <summary>
    /// Gets or sets the service expectation percentage.
    /// </summary>
    public decimal? ServiceExpectationPercentage { get; set; }

    /// <summary>
    /// Gets or sets the reason for employment change (Promotion, Transfer, New Hire, Retirement, etc.).
    /// </summary>
    [MaxLength(200)]
    public string? ChangeReason { get; set; }

    /// <summary>
    /// Gets or sets additional notes about this employment period.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets who approved this employment change.
    /// </summary>
    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// Gets or sets the date this employment record was approved.
    /// </summary>
    public DateTime? ApprovalDate { get; set; }

    /// <summary>
    /// Navigation property to the academic employee.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }
}