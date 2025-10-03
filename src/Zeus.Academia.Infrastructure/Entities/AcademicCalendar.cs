using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic calendar with important dates and events.
/// This is a completely new entity that doesn't modify existing functionality.
/// </summary>
public class AcademicCalendar : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the calendar name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CalendarName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the academic year this calendar applies to.
    /// </summary>
    [Required]
    public int AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the calendar description.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the calendar start date.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the calendar end date.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this calendar is currently active.
    /// </summary>
    public bool IsPublished { get; set; } = false;

    /// <summary>
    /// Navigation property for calendar events.
    /// </summary>
    public virtual ICollection<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();
}

/// <summary>
/// Entity representing an event in the academic calendar.
/// </summary>
public class CalendarEvent : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the calendar ID this event belongs to.
    /// </summary>
    [Required]
    public int CalendarId { get; set; }

    /// <summary>
    /// Gets or sets the event name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event description.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the event start date.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the event end date (optional for single-day events).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the event type (using existing enum to avoid conflicts).
    /// </summary>
    public EventType EventType { get; set; } = EventType.Academic;

    /// <summary>
    /// Gets or sets whether this is an all-day event.
    /// </summary>
    public bool IsAllDay { get; set; } = true;

    /// <summary>
    /// Navigation property to the calendar.
    /// </summary>
    public virtual AcademicCalendar Calendar { get; set; } = null!;
}

/// <summary>
/// Enumeration for calendar event types (new enum that doesn't conflict).
/// </summary>
public enum EventType
{
    Academic = 1,
    Registration = 2,
    Holiday = 3,
    Examination = 4,
    Administrative = 5,
    Special = 6
}