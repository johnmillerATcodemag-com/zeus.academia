using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing the assignment of a student to an academic advisor
/// </summary>
public class StudentAdvisorAssignment : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the assignment.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the student's employee number.
    /// </summary>
    public int StudentEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the advisor's ID.
    /// </summary>
    public int AdvisorId { get; set; }

    /// <summary>
    /// Gets or sets when the assignment started.
    /// </summary>
    public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when the assignment ended (if no longer active).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this assignment is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this is the primary advisor for the student.
    /// </summary>
    public bool IsPrimary { get; set; } = true;

    /// <summary>
    /// Gets or sets the type of advisory relationship.
    /// </summary>
    public AdvisorType AdvisorType { get; set; } = AdvisorType.Academic;

    /// <summary>
    /// Gets or sets the reason for the assignment.
    /// </summary>
    [MaxLength(500)]
    public string? AssignmentReason { get; set; }

    /// <summary>
    /// Gets or sets the reason for ending the assignment (if ended).
    /// </summary>
    [MaxLength(500)]
    public string? EndReason { get; set; }

    /// <summary>
    /// Gets or sets who made the assignment.
    /// </summary>
    [MaxLength(100)]
    public string? AssignedBy { get; set; }

    /// <summary>
    /// Gets or sets any notes about this assignment.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    public virtual Student? Student { get; set; }

    /// <summary>
    /// Navigation property to the academic advisor.
    /// </summary>
    public virtual AcademicAdvisor? Advisor { get; set; }
}