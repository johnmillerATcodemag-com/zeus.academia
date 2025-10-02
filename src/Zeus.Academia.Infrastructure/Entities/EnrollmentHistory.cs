using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing the historical record of enrollment events and status changes
/// </summary>
public class EnrollmentHistory : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the history record.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the student's employee number.
    /// </summary>
    [Required]
    public int StudentEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the application ID if this event is related to an application.
    /// </summary>
    public int? ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the type of enrollment event.
    /// </summary>
    [Required]
    public EnrollmentEventType EventType { get; set; }

    /// <summary>
    /// Gets or sets the previous enrollment status (if applicable).
    /// </summary>
    public EnrollmentStatus? PreviousStatus { get; set; }

    /// <summary>
    /// Gets or sets the new enrollment status.
    /// </summary>
    [Required]
    public EnrollmentStatus NewStatus { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the event occurred.
    /// </summary>
    [Required]
    public DateTime EventDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the academic term when the event occurred.
    /// </summary>
    [MaxLength(20)]
    public string? AcademicTerm { get; set; }

    /// <summary>
    /// Gets or sets the academic year.
    /// </summary>
    public int? AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the reason for the status change or event.
    /// </summary>
    [MaxLength(1000)]
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the event.
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets who initiated or processed this event.
    /// </summary>
    [MaxLength(100)]
    public string? ProcessedBy { get; set; }

    /// <summary>
    /// Gets or sets the department involved in the event.
    /// </summary>
    [MaxLength(100)]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the program associated with the enrollment.
    /// </summary>
    [MaxLength(100)]
    public string? Program { get; set; }

    /// <summary>
    /// Gets or sets whether this is a system-generated event.
    /// </summary>
    public bool IsSystemGenerated { get; set; } = false;

    /// <summary>
    /// Gets or sets whether notifications were sent for this event.
    /// </summary>
    public bool NotificationSent { get; set; } = false;

    /// <summary>
    /// Gets or sets the date when notifications were sent.
    /// </summary>
    public DateTime? NotificationDate { get; set; }

    /// <summary>
    /// Gets or sets the effective date when the status change takes effect (if different from event date).
    /// </summary>
    public DateTime? EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets additional metadata as JSON string.
    /// </summary>
    [MaxLength(2000)]
    public string? Metadata { get; set; }

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    public virtual Student Student { get; set; } = null!;

    /// <summary>
    /// Navigation property to the enrollment application (if applicable).
    /// </summary>
    public virtual EnrollmentApplication? Application { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }
}