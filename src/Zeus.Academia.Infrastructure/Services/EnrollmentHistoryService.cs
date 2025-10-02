using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for managing enrollment history tracking
/// </summary>
public class EnrollmentHistoryService : IEnrollmentHistoryService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<EnrollmentHistoryService> _logger;

    public EnrollmentHistoryService(AcademiaDbContext context, ILogger<EnrollmentHistoryService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<EnrollmentHistory> RecordEnrollmentEventAsync(
        int studentEmpNr,
        EnrollmentEventType eventType,
        EnrollmentStatus newStatus,
        EnrollmentStatus? previousStatus = null,
        string? reason = null,
        string? notes = null,
        string? processedBy = null,
        int? applicationId = null,
        string? departmentName = null,
        string? program = null,
        string? academicTerm = null,
        int? academicYear = null)
    {
        _logger.LogInformation("Recording enrollment event for student {StudentEmpNr}: {EventType}",
            studentEmpNr, eventType);

        var historyRecord = new EnrollmentHistory
        {
            StudentEmpNr = studentEmpNr,
            ApplicationId = applicationId,
            EventType = eventType,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            EventDate = DateTime.UtcNow,
            AcademicTerm = academicTerm,
            AcademicYear = academicYear,
            Reason = reason,
            Notes = notes,
            ProcessedBy = processedBy ?? "System",
            DepartmentName = departmentName,
            Program = program,
            IsSystemGenerated = string.IsNullOrWhiteSpace(processedBy) || processedBy == "System",
            NotificationSent = false,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        _context.EnrollmentHistory.Add(historyRecord);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully recorded enrollment event with ID: {HistoryId}", historyRecord.Id);
        return historyRecord;
    }

    public async Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetStudentEnrollmentHistoryAsync(
        int studentEmpNr, int pageNumber = 1, int pageSize = 10)
    {
        _logger.LogDebug("Getting enrollment history for student {StudentEmpNr}", studentEmpNr);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _context.EnrollmentHistory
            .Include(eh => eh.Student)
            .Include(eh => eh.Application)
            .Include(eh => eh.Department)
            .Where(eh => eh.StudentEmpNr == studentEmpNr);

        var totalCount = await query.CountAsync();

        var history = await query
            .OrderByDescending(eh => eh.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (history, totalCount);
    }

    public async Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryByEventTypeAsync(
        EnrollmentEventType eventType, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.EnrollmentHistory
            .Include(eh => eh.Student)
            .Include(eh => eh.Application)
            .Where(eh => eh.EventType == eventType);

        var totalCount = await query.CountAsync();

        var history = await query
            .OrderByDescending(eh => eh.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (history, totalCount);
    }

    public async Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryByDateRangeAsync(
        DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.EnrollmentHistory
            .Include(eh => eh.Student)
            .Include(eh => eh.Application)
            .Where(eh => eh.EventDate >= startDate && eh.EventDate <= endDate);

        var totalCount = await query.CountAsync();

        var history = await query
            .OrderByDescending(eh => eh.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (history, totalCount);
    }

    public async Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryByDepartmentAsync(
        string departmentName, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.EnrollmentHistory
            .Include(eh => eh.Student)
            .Include(eh => eh.Application)
            .Include(eh => eh.Department)
            .Where(eh => eh.DepartmentName == departmentName);

        var totalCount = await query.CountAsync();

        var history = await query
            .OrderByDescending(eh => eh.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (history, totalCount);
    }

    public async Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryByTermAsync(
        string academicTerm, int academicYear, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.EnrollmentHistory
            .Include(eh => eh.Student)
            .Include(eh => eh.Application)
            .Where(eh => eh.AcademicTerm == academicTerm && eh.AcademicYear == academicYear);

        var totalCount = await query.CountAsync();

        var history = await query
            .OrderByDescending(eh => eh.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (history, totalCount);
    }

    public async Task<EnrollmentHistory?> GetLatestEnrollmentEventAsync(int studentEmpNr)
    {
        return await _context.EnrollmentHistory
            .Include(eh => eh.Student)
            .Include(eh => eh.Application)
            .Where(eh => eh.StudentEmpNr == studentEmpNr)
            .OrderByDescending(eh => eh.EventDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<EnrollmentHistory>> GetApplicationHistoryAsync(int applicationId)
    {
        return await _context.EnrollmentHistory
            .Include(eh => eh.Student)
            .Include(eh => eh.Application)
            .Where(eh => eh.ApplicationId == applicationId)
            .OrderByDescending(eh => eh.EventDate)
            .ToListAsync();
    }

    public async Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> SearchHistoryAsync(
        int? studentEmpNr = null,
        EnrollmentEventType? eventType = null,
        EnrollmentStatus? status = null,
        string? departmentName = null,
        string? program = null,
        string? academicTerm = null,
        int? academicYear = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        _logger.LogDebug("Searching enrollment history with criteria");

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _context.EnrollmentHistory
            .Include(eh => eh.Student)
            .Include(eh => eh.Application)
            .Include(eh => eh.Department)
            .AsQueryable();

        // Apply filters
        if (studentEmpNr.HasValue)
        {
            query = query.Where(eh => eh.StudentEmpNr == studentEmpNr.Value);
        }

        if (eventType.HasValue)
        {
            query = query.Where(eh => eh.EventType == eventType.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(eh => eh.NewStatus == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(departmentName))
        {
            query = query.Where(eh => eh.DepartmentName == departmentName);
        }

        if (!string.IsNullOrWhiteSpace(program))
        {
            query = query.Where(eh => eh.Program == program);
        }

        if (!string.IsNullOrWhiteSpace(academicTerm))
        {
            query = query.Where(eh => eh.AcademicTerm == academicTerm);
        }

        if (academicYear.HasValue)
        {
            query = query.Where(eh => eh.AcademicYear == academicYear.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(eh => eh.EventDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(eh => eh.EventDate <= endDate.Value);
        }

        var totalCount = await query.CountAsync();

        var history = await query
            .OrderByDescending(eh => eh.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (history, totalCount);
    }

    public async Task<Dictionary<EnrollmentEventType, int>> GetEnrollmentStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var statistics = await _context.EnrollmentHistory
            .Where(eh => eh.EventDate >= startDate && eh.EventDate <= endDate)
            .GroupBy(eh => eh.EventType)
            .Select(g => new { EventType = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EventType, x => x.Count);

        return statistics;
    }

    public async Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryRequiringNotificationAsync(
        int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.EnrollmentHistory
            .Include(eh => eh.Student)
            .Include(eh => eh.Application)
            .Where(eh => !eh.NotificationSent &&
                        (eh.EventType == EnrollmentEventType.AdmissionDecision ||
                         eh.EventType == EnrollmentEventType.Enrolled ||
                         eh.EventType == EnrollmentEventType.StatusChanged ||
                         eh.EventType == EnrollmentEventType.Graduated));

        var totalCount = await query.CountAsync();

        var history = await query
            .OrderBy(eh => eh.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (history, totalCount);
    }

    public async Task<bool> MarkNotificationSentAsync(int historyId)
    {
        var historyRecord = await _context.EnrollmentHistory.FindAsync(historyId);
        if (historyRecord == null) return false;

        historyRecord.NotificationSent = true;
        historyRecord.NotificationDate = DateTime.UtcNow;
        historyRecord.ModifiedBy = "System";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetTotalHistoryCountAsync()
    {
        return await _context.EnrollmentHistory.CountAsync();
    }

    public async Task<(IEnumerable<EnrollmentHistory> History, int TotalCount)> GetHistoryForStudentsAsync(
        IEnumerable<int> studentEmpNrs, int pageNumber = 1, int pageSize = 10)
    {
        var studentEmpNrsList = studentEmpNrs.ToList();
        if (!studentEmpNrsList.Any())
        {
            return (Enumerable.Empty<EnrollmentHistory>(), 0);
        }

        var query = _context.EnrollmentHistory
            .Include(eh => eh.Student)
            .Include(eh => eh.Application)
            .Where(eh => studentEmpNrsList.Contains(eh.StudentEmpNr));

        var totalCount = await query.CountAsync();

        var history = await query
            .OrderByDescending(eh => eh.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (history, totalCount);
    }
}