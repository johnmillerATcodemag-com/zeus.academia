using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Api.UnitTests.Entities;

/// <summary>
/// Unit tests for AcademicCalendar entity.
/// These are NEW tests that demonstrate adding entities without breaking existing functionality.
/// </summary>
public class AcademicCalendarTests
{
    [Fact]
    public void AcademicCalendar_Should_Have_Valid_Properties()
    {
        // Arrange & Act
        var calendar = new AcademicCalendar
        {
            Id = 1,
            CalendarName = "2025-2026 Academic Calendar",
            AcademicYear = 2025,
            Description = "Official academic calendar for the 2025-2026 academic year",
            StartDate = new DateTime(2025, 8, 15),
            EndDate = new DateTime(2026, 5, 15),
            IsPublished = true
        };

        // Assert
        Assert.Equal(1, calendar.Id);
        Assert.Equal("2025-2026 Academic Calendar", calendar.CalendarName);
        Assert.Equal(2025, calendar.AcademicYear);
        Assert.Equal("Official academic calendar for the 2025-2026 academic year", calendar.Description);
        Assert.Equal(new DateTime(2025, 8, 15), calendar.StartDate);
        Assert.Equal(new DateTime(2026, 5, 15), calendar.EndDate);
        Assert.True(calendar.IsPublished);
    }

    [Fact]
    public void AcademicCalendar_Should_Support_Events()
    {
        // Arrange
        var calendar = new AcademicCalendar
        {
            CalendarName = "2025-2026 Calendar",
            AcademicYear = 2025,
            StartDate = new DateTime(2025, 8, 15),
            EndDate = new DateTime(2026, 5, 15)
        };

        var fallStart = new CalendarEvent
        {
            CalendarId = calendar.Id,
            EventName = "Fall Semester Begins",
            Description = "First day of fall semester classes",
            StartDate = new DateTime(2025, 8, 22),
            EventType = EventType.Academic,
            IsAllDay = true
        };

        var thanksgiving = new CalendarEvent
        {
            CalendarId = calendar.Id,
            EventName = "Thanksgiving Break",
            Description = "University closed for Thanksgiving",
            StartDate = new DateTime(2025, 11, 28),
            EndDate = new DateTime(2025, 11, 29),
            EventType = EventType.Holiday,
            IsAllDay = true
        };

        // Act
        calendar.Events.Add(fallStart);
        calendar.Events.Add(thanksgiving);

        // Assert
        Assert.Equal(2, calendar.Events.Count);
        Assert.Contains(fallStart, calendar.Events);
        Assert.Contains(thanksgiving, calendar.Events);
        Assert.Equal(EventType.Academic, fallStart.EventType);
        Assert.Equal(EventType.Holiday, thanksgiving.EventType);
    }

    [Fact]
    public void CalendarEvent_Should_Support_Multi_Day_Events()
    {
        // Arrange & Act
        var finalExams = new CalendarEvent
        {
            Id = 1,
            CalendarId = 1,
            EventName = "Final Examinations",
            Description = "Final examination period for fall semester",
            StartDate = new DateTime(2025, 12, 10),
            EndDate = new DateTime(2025, 12, 17),
            EventType = EventType.Examination,
            IsAllDay = true
        };

        // Assert
        Assert.Equal("Final Examinations", finalExams.EventName);
        Assert.Equal(EventType.Examination, finalExams.EventType);
        Assert.True(finalExams.EndDate.HasValue);
        Assert.True(finalExams.EndDate > finalExams.StartDate);
        Assert.True(finalExams.IsAllDay);
    }

    [Fact]
    public void CalendarEvent_Should_Support_Single_Day_Events()
    {
        // Arrange & Act
        var graduation = new CalendarEvent
        {
            Id = 2,
            CalendarId = 1,
            EventName = "Commencement Ceremony",
            Description = "Spring commencement ceremony",
            StartDate = new DateTime(2026, 5, 15),
            EndDate = null, // Single day event
            EventType = EventType.Special,
            IsAllDay = false // Specific time event
        };

        // Assert
        Assert.Equal("Commencement Ceremony", graduation.EventName);
        Assert.Equal(EventType.Special, graduation.EventType);
        Assert.False(graduation.EndDate.HasValue);
        Assert.False(graduation.IsAllDay);
    }
}