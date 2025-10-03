using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Course waitlist service implementation
/// </summary>
public class CourseWaitlistService : ICourseWaitlistService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<CourseWaitlistService> _logger;

    public CourseWaitlistService(AcademiaDbContext context, ILogger<CourseWaitlistService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Interface method implementation
    public async Task<CourseWaitlistResult> AddToWaitlistAsync(CourseWaitlistRequest request)
    {
        var waitlistRequest = new WaitlistRequest
        {
            StudentId = request.StudentId,
            SectionId = request.CourseOfferingId, // Map CourseOfferingId to SectionId
            Priority = request.Priority,
            NotificationPreferences = request.NotificationPreferences,
            AutoEnroll = request.AutoEnroll,
            ExpirationDate = request.ExpirationDate
        };

        var entry = await AddToWaitlistInternalAsync(waitlistRequest);

        return new CourseWaitlistResult
        {
            Success = true,
            WaitlistPosition = entry.Position,
            Status = entry.Status,
            Message = $"Successfully added to waitlist at position {entry.Position}",
            CreatedAt = entry.RequestDate
        };
    }

    public async Task<WaitlistEntry> AddToWaitlistInternalAsync(WaitlistRequest request)
    {
        try
        {
            _logger.LogInformation("Adding student {StudentId} to waitlist for section {SectionId}",
                request.StudentId, request.SectionId);

            // Verify section exists and is full
            var section = await _context.CourseOfferings
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Id == request.SectionId);

            if (section == null)
            {
                throw new ArgumentException($"Section {request.SectionId} not found");
            }

            // Check if section is actually full
            var currentEnrollment = await _context.CourseEnrollments
                .CountAsync(e => e.SubjectCode == section.Course.SubjectCode &&
                               e.Status == CourseEnrollmentStatus.Enrolled);

            if (currentEnrollment < (section.MaxEnrollment ?? 30))
            {
                throw new InvalidOperationException("Cannot join waitlist - section has available seats");
            }

            // Check if student is already on waitlist
            var existingEntry = await _context.Set<WaitlistEntry>()
                .FirstOrDefaultAsync(w => w.StudentId == request.StudentId &&
                                        w.SectionId == request.SectionId &&
                                        w.Status == WaitlistStatus.Active);

            if (existingEntry != null)
            {
                throw new InvalidOperationException("Student is already on the waitlist for this section");
            }

            // Calculate priority and position
            var priority = CalculateWaitlistPriority(request);
            var position = await GetNextWaitlistPosition(request.SectionId, priority);

            var waitlistEntry = new WaitlistEntry
            {
                StudentId = request.StudentId,
                SectionId = request.SectionId,
                Priority = priority,
                Position = position,
                Status = WaitlistStatus.Active,
                RequestDate = DateTime.UtcNow,
                NotificationPreferences = request.NotificationPreferences ?? new NotificationPreferences
                {
                    EmailNotifications = true,
                    SMSNotifications = false,
                    PushNotifications = true
                },
                AutoEnroll = request.AutoEnroll,
                ExpirationDate = request.ExpirationDate ?? DateTime.UtcNow.AddDays(30)
            };

            _context.Set<WaitlistEntry>().Add(waitlistEntry);
            await _context.SaveChangesAsync();

            // Send confirmation notification
            await SendWaitlistConfirmation(waitlistEntry);

            _logger.LogInformation("Student {StudentId} added to waitlist for section {SectionId} at position {Position}",
                request.StudentId, request.SectionId, position);

            return waitlistEntry;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding student {StudentId} to waitlist for section {SectionId}",
                request.StudentId, request.SectionId);
            throw;
        }
    }

    public async Task<bool> RemoveFromWaitlistAsync(int studentId, int courseOfferingId)
    {
        try
        {
            _logger.LogInformation("Removing student {StudentId} from waitlist for course offering {CourseOfferingId}",
                studentId, courseOfferingId);

            var waitlistEntry = await _context.Set<WaitlistEntry>()
                .FirstOrDefaultAsync(w => w.StudentId == studentId &&
                                        w.SectionId == courseOfferingId &&
                                        w.Status == WaitlistStatus.Active);

            if (waitlistEntry == null)
            {
                _logger.LogWarning("No active waitlist entry found for student {StudentId} and course offering {CourseOfferingId}",
                    studentId, courseOfferingId);
                return false;
            }

            waitlistEntry.Status = WaitlistStatus.Removed;
            waitlistEntry.RemovalDate = DateTime.UtcNow;
            waitlistEntry.RemovalReason = "Student requested removal";

            // Reorder remaining waitlist entries
            await ReorderWaitlistAfterRemoval(waitlistEntry.SectionId, waitlistEntry.Position);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Student {StudentId} removed from waitlist for course offering {CourseOfferingId}",
                studentId, courseOfferingId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing student {StudentId} from waitlist for course offering {CourseOfferingId}",
                studentId, courseOfferingId);
            throw;
        }
    }

    public async Task<List<CourseWaitlistEntry>> GetWaitlistStatusAsync(int studentId)
    {
        try
        {
            _logger.LogInformation("Getting waitlist status for student {StudentId}", studentId);

            var waitlistEntries = await _context.Set<WaitlistEntry>()
                .Where(w => w.StudentId == studentId && w.Status == WaitlistStatus.Active)
                .Include(w => w.Section)
                .ThenInclude(s => s.CourseOffering)
                .ThenInclude(o => o.Course)
                .OrderBy(w => w.Position)
                .ToListAsync();

            // Convert WaitlistEntry entities to CourseWaitlistEntry models
            var results = waitlistEntries.Select(w => new CourseWaitlistEntry
            {
                Id = w.Id,
                StudentId = w.StudentId,
                CourseId = w.Section?.CourseOffering?.Course?.Id ?? 0,
                CourseOfferingId = w.SectionId,
                WaitlistPosition = w.Position,
                Priority = w.Priority,
                Status = w.Status,
                CreatedAt = w.RequestDate,
                ProcessedAt = w.EnrollmentDate,
                NotificationPreferences = w.NotificationPreferences
            }).ToList();

            _logger.LogInformation("Found {Count} active waitlist entries for student {StudentId}",
                results.Count, studentId);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waitlist status for student {StudentId}", studentId);
            throw;
        }
    }

    public async Task<WaitlistSummary> GetWaitlistSummaryAsync(int sectionId)
    {
        try
        {
            _logger.LogInformation("Getting waitlist summary for section {SectionId}", sectionId);

            var section = await _context.CourseOfferings
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Id == sectionId);

            if (section == null)
            {
                throw new ArgumentException($"Section {sectionId} not found");
            }

            var waitlistEntries = await _context.Set<WaitlistEntry>()
                .Where(w => w.SectionId == sectionId && w.Status == WaitlistStatus.Active)
                .OrderBy(w => w.Priority)
                .ThenBy(w => w.RequestDate)
                .ToListAsync();

            var currentEnrollment = await _context.CourseEnrollments
                .CountAsync(e => e.SubjectCode == section.Course.SubjectCode &&
                               e.Status == CourseEnrollmentStatus.Enrolled);

            var summary = new WaitlistSummary
            {
                SectionId = sectionId,
                CourseTitle = section.Course.Title,
                SectionCode = section.SectionId,
                MaxCapacity = section.MaxEnrollment ?? 30, // Default capacity
                CurrentEnrollment = currentEnrollment,
                AvailableSeats = Math.Max(0, (section.MaxEnrollment ?? 30) - currentEnrollment),
                WaitlistCount = waitlistEntries.Count,
                WaitlistEntries = waitlistEntries,
                EstimatedProcessingTime = CalculateEstimatedProcessingTime(waitlistEntries.Count),
                LastUpdated = DateTime.UtcNow
            };

            _logger.LogInformation("Waitlist summary generated for section {SectionId}: {WaitlistCount} entries",
                sectionId, waitlistEntries.Count);

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waitlist summary for section {SectionId}", sectionId);
            throw;
        }
    }

    public async Task ProcessWaitlistAsync(int sectionId)
    {
        try
        {
            _logger.LogInformation("Processing waitlist for section {SectionId}", sectionId);

            var section = await _context.CourseOfferings
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Id == sectionId);

            if (section == null)
            {
                _logger.LogWarning("Section {SectionId} not found", sectionId);
                return;
            }

            // Get current enrollment count
            var currentEnrollment = await _context.CourseEnrollments
                .CountAsync(e => e.SubjectCode == section.Course.SubjectCode &&
                               e.Status == CourseEnrollmentStatus.Enrolled);

            var availableSeats = (section.MaxEnrollment ?? 30) - currentEnrollment;

            if (availableSeats <= 0)
            {
                _logger.LogInformation("No available seats in section {SectionId}", sectionId);
                return;
            }

            // Get prioritized waitlist entries
            var waitlistEntries = await _context.Set<WaitlistEntry>()
                .Where(w => w.SectionId == sectionId && w.Status == WaitlistStatus.Active)
                .OrderBy(w => w.Priority)
                .ThenBy(w => w.RequestDate)
                .Take(availableSeats)
                .ToListAsync();

            var processedCount = 0;

            foreach (var entry in waitlistEntries)
            {
                try
                {
                    var success = await ProcessSingleWaitlistEntry(entry);
                    if (success)
                    {
                        processedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing waitlist entry {EntryId}", entry.Id);
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Processed {ProcessedCount} waitlist entries for section {SectionId}",
                processedCount, sectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing waitlist for section {SectionId}", sectionId);
            throw;
        }
    }

    public async Task SendWaitlistNotificationsAsync()
    {
        try
        {
            _logger.LogInformation("Sending waitlist notifications");

            // Get entries that need position updates
            var entriesToNotify = await _context.Set<WaitlistEntry>()
                .Where(w => w.Status == WaitlistStatus.Active &&
                           w.LastNotificationDate == null ||
                           w.LastNotificationDate < DateTime.UtcNow.AddDays(-1))
                .Include(w => w.Section)
                .ThenInclude(s => s.CourseOffering)
                .ThenInclude(o => o.Course)
                .ToListAsync();

            var notificationsSent = 0;

            foreach (var entry in entriesToNotify)
            {
                try
                {
                    await SendPositionUpdateNotification(entry);
                    entry.LastNotificationDate = DateTime.UtcNow;
                    notificationsSent++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending notification for waitlist entry {EntryId}", entry.Id);
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Sent {NotificationCount} waitlist notifications", notificationsSent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending waitlist notifications");
            throw;
        }
    }

    #region Private Helper Methods

    private WaitlistPriority CalculateWaitlistPriority(WaitlistRequest request)
    {
        // Default priority logic - can be enhanced based on business rules
        var priority = WaitlistPriority.Normal;

        // Higher priority for seniors
        if (request.StudentClassStanding == ClassStanding.Senior)
            priority = WaitlistPriority.High;

        // Special priority for degree requirements
        if (request.IsRequiredForDegree)
            priority = WaitlistPriority.High;

        // Graduating seniors get highest priority
        if (request.StudentClassStanding == ClassStanding.Senior && request.IsGraduatingStudent)
            priority = WaitlistPriority.Emergency;

        return priority;
    }

    private async Task<int> GetNextWaitlistPosition(int sectionId, WaitlistPriority priority)
    {
        var existingEntries = await _context.Set<WaitlistEntry>()
            .Where(w => w.SectionId == sectionId && w.Status == WaitlistStatus.Active)
            .Where(w => w.Priority <= priority) // Same or higher priority
            .CountAsync();

        return existingEntries + 1;
    }

    private async Task ReorderWaitlistAfterRemoval(int sectionId, int removedPosition)
    {
        var entriesToReorder = await _context.Set<WaitlistEntry>()
            .Where(w => w.SectionId == sectionId &&
                       w.Status == WaitlistStatus.Active &&
                       w.Position > removedPosition)
            .ToListAsync();

        foreach (var entry in entriesToReorder)
        {
            entry.Position--;
        }
    }

    private async Task<bool> ProcessSingleWaitlistEntry(WaitlistEntry entry)
    {
        try
        {
            if (entry.AutoEnroll)
            {
                // Attempt automatic enrollment
                var enrollment = new StudentEnrollment
                {
                    StudentEmpNr = entry.StudentId,
                    SubjectCode = "TEMP", // Need to get from context
                    EnrollmentDate = DateTime.UtcNow,
                    Status = "Enrolled",
                    Grade = string.Empty
                };

                _context.StudentEnrollments.Add(enrollment);

                entry.Status = WaitlistStatus.Enrolled;
                entry.EnrollmentDate = DateTime.UtcNow;

                await SendEnrollmentNotification(entry);
                return true;
            }
            else
            {
                // Send opportunity notification
                entry.Status = WaitlistStatus.Active;
                entry.OfferDate = DateTime.UtcNow;
                entry.OfferExpirationDate = DateTime.UtcNow.AddHours(24); // 24-hour response window

                await SendEnrollmentOpportunityNotification(entry);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing waitlist entry {EntryId}", entry.Id);
            return false;
        }
    }

    private TimeSpan CalculateEstimatedProcessingTime(int waitlistPosition)
    {
        // Estimate based on historical data and position
        // This is a simplified calculation
        var daysPerPosition = 3; // Average days for each position to move up
        var estimatedDays = waitlistPosition * daysPerPosition;

        return TimeSpan.FromDays(Math.Min(estimatedDays, 90)); // Cap at 90 days
    }

    #endregion

    #region Notification Methods

    private async Task SendWaitlistConfirmation(WaitlistEntry entry)
    {
        _logger.LogInformation("Sending waitlist confirmation to student {StudentId} for section {SectionId}",
            entry.StudentId, entry.SectionId);

        // Implementation would send actual notifications
        // For now, just log the action
        await Task.CompletedTask;
    }

    private async Task SendPositionUpdateNotification(WaitlistEntry entry)
    {
        _logger.LogInformation("Sending position update notification to student {StudentId}: position {Position}",
            entry.StudentId, entry.Position);

        // Implementation would send actual notifications
        await Task.CompletedTask;
    }

    private async Task SendEnrollmentNotification(WaitlistEntry entry)
    {
        _logger.LogInformation("Sending enrollment notification to student {StudentId} for section {SectionId}",
            entry.StudentId, entry.SectionId);

        // Implementation would send actual notifications
        await Task.CompletedTask;
    }

    private async Task SendEnrollmentOpportunityNotification(WaitlistEntry entry)
    {
        _logger.LogInformation("Sending enrollment opportunity notification to student {StudentId} for section {SectionId}",
            entry.StudentId, entry.SectionId);

        // Implementation would send actual notifications
        await Task.CompletedTask;
    }

    #endregion



    public async Task<List<WaitlistProcessingResult>> ProcessWaitlistAsync(int courseOfferingId, int availableSeats)
    {
        await ProcessWaitlistAsync(courseOfferingId);

        // Return mock results for now
        return new List<WaitlistProcessingResult>();
    }

    public async Task<NotificationResult> SendWaitlistNotificationAsync(CourseWaitlistEntry entry, NotificationType notificationType)
    {
        try
        {
            _logger.LogInformation("Sending waitlist notification of type {NotificationType} to student {StudentId}",
                notificationType, entry.StudentId);

            // Mock notification sending
            await Task.Delay(100);

            return new NotificationResult
            {
                Success = true,
                Message = "Notification sent successfully",
                NotificationType = notificationType,
                SentAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending waitlist notification");
            return new NotificationResult
            {
                Success = false,
                Message = ex.Message,
                NotificationType = notificationType,
                SentAt = DateTime.UtcNow
            };
        }
    }

    public async Task<List<CourseWaitlistEntry>> GetStudentWaitlistStatusAsync(int studentId)
    {
        try
        {
            // GetWaitlistStatusAsync already returns List<CourseWaitlistEntry>
            return await GetWaitlistStatusAsync(studentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student waitlist status");
            return new List<CourseWaitlistEntry>();
        }
    }

    public async Task<WaitlistStatistics> GetWaitlistStatisticsAsync(int courseOfferingId)
    {
        try
        {
            var summary = await GetWaitlistSummaryAsync(courseOfferingId);

            return new WaitlistStatistics
            {
                TotalWaitlisted = summary.WaitlistCount,
                AverageWaitTime = (int)summary.EstimatedProcessingTime.TotalDays,
                EstimatedSeatsAvailable = summary.AvailableSeats,
                PriorityBreakdown = summary.WaitlistEntries
                    .GroupBy(w => w.Priority)
                    .ToDictionary(g => g.Key, g => g.Count()),
                HistoricalEnrollmentRate = 0.75f // Mock data
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waitlist statistics");
            return new WaitlistStatistics();
        }
    }
}