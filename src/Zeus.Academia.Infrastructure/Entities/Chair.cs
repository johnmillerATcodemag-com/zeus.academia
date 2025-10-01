using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a Chair in the academic system.
/// A Chair is both an Academic and has chair-specific properties.
/// Based on the ORM model, Chair has its own identity separate from Academic.
/// </summary>
public class Chair : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the chair - the primary identifier.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the chair position.
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the department that this chair belongs to.
    /// </summary>
    [MaxLength(15)]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the academic who holds this chair position.
    /// </summary>
    public int? AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the start date of the chair appointment.
    /// </summary>
    public DateTime? AppointmentStartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the chair appointment.
    /// </summary>
    public DateTime? AppointmentEndDate { get; set; }

    /// <summary>
    /// Gets or sets whether the chair position is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to the academic who holds this chair.
    /// </summary>
    public virtual Academic? Academic { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }
}