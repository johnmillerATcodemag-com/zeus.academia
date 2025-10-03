using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Configuration;

/// <summary>
/// Entity Framework configuration for AcademicCalendar entity.
/// This configuration is completely additive and doesn't affect existing entities.
/// </summary>
public class AcademicCalendarConfiguration : IEntityTypeConfiguration<AcademicCalendar>
{
    public void Configure(EntityTypeBuilder<AcademicCalendar> builder)
    {
        builder.ToTable("AcademicCalendars");

        builder.HasKey(ac => ac.Id);

        builder.Property(ac => ac.CalendarName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ac => ac.AcademicYear)
            .IsRequired();

        builder.Property(ac => ac.Description)
            .HasMaxLength(1000);

        builder.Property(ac => ac.StartDate)
            .IsRequired();

        builder.Property(ac => ac.EndDate)
            .IsRequired();

        builder.Property(ac => ac.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

        // One-to-many relationship with calendar events
        builder.HasMany(ac => ac.Events)
            .WithOne(ce => ce.Calendar)
            .HasForeignKey(ce => ce.CalendarId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(ac => ac.AcademicYear)
            .HasDatabaseName("IX_AcademicCalendars_AcademicYear");

        builder.HasIndex(ac => ac.IsPublished)
            .HasDatabaseName("IX_AcademicCalendars_IsPublished");

        builder.HasIndex(ac => new { ac.AcademicYear, ac.IsPublished })
            .HasDatabaseName("IX_AcademicCalendars_AcademicYear_IsPublished");

        // Ensure unique calendar name per academic year
        builder.HasIndex(ac => new { ac.CalendarName, ac.AcademicYear })
            .IsUnique()
            .HasDatabaseName("IX_AcademicCalendars_CalendarName_AcademicYear");
    }
}

/// <summary>
/// Entity Framework configuration for CalendarEvent entity.
/// </summary>
public class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("CalendarEvents");

        builder.HasKey(ce => ce.Id);

        builder.Property(ce => ce.CalendarId)
            .IsRequired();

        builder.Property(ce => ce.EventName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ce => ce.Description)
            .HasMaxLength(500);

        builder.Property(ce => ce.StartDate)
            .IsRequired();

        builder.Property(ce => ce.EventType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ce => ce.IsAllDay)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes for performance
        builder.HasIndex(ce => ce.CalendarId)
            .HasDatabaseName("IX_CalendarEvents_CalendarId");

        builder.HasIndex(ce => ce.StartDate)
            .HasDatabaseName("IX_CalendarEvents_StartDate");

        builder.HasIndex(ce => ce.EventType)
            .HasDatabaseName("IX_CalendarEvents_EventType");

        builder.HasIndex(ce => new { ce.CalendarId, ce.StartDate })
            .HasDatabaseName("IX_CalendarEvents_CalendarId_StartDate");

        // Check constraints
        builder.ToTable("CalendarEvents", t =>
        {
            t.HasCheckConstraint("CK_CalendarEvents_EndDate", "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
        });
    }
}