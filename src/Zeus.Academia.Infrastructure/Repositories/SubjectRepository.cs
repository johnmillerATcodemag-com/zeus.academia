using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Subject entities with specialized operations.
/// </summary>
public class SubjectRepository : Repository<Subject>, ISubjectRepository
{
    /// <summary>
    /// Initializes a new instance of the SubjectRepository class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    public SubjectRepository(AcademiaDbContext context, ILogger<SubjectRepository> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Subject>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(departmentName);
            _logger.LogDebug("Getting subjects by department name {DepartmentName}", departmentName);

            return await _dbSet
                .Where(s => s.DepartmentName == departmentName)
                .Include(s => s.Department)
                .OrderBy(s => s.Title)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subjects by department name {DepartmentName}", departmentName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Subject?> GetByCodeAsync(string subjectCode, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(subjectCode);
            _logger.LogDebug("Getting subject by code {SubjectCode}", subjectCode);

            return await _dbSet
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Code == subjectCode, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subject by code {SubjectCode}", subjectCode);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Subject>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(namePattern);
            _logger.LogDebug("Searching subjects by name pattern {NamePattern}", namePattern);

            var pattern = $"%{namePattern}%";
            return await _dbSet
                .Where(s => EF.Functions.Like(s.Title, pattern))
                .Include(s => s.Department)
                .OrderBy(s => s.Title)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching subjects by name pattern {NamePattern}", namePattern);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Subject>> GetByCreditRangeAsync(int minCredits, int maxCredits, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting subjects by credit range {MinCredits}-{MaxCredits}", minCredits, maxCredits);
            return await _dbSet
                .Where(s => s.CreditHours >= minCredits && s.CreditHours <= maxCredits)
                .Include(s => s.Department)
                .OrderBy(s => s.CreditHours)
                .ThenBy(s => s.Title)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subjects by credit range {MinCredits}-{MaxCredits}", minCredits, maxCredits);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Subject>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting subjects by teacher ID {TeacherId}", teacherId);
            return await _dbSet
                .Where(s => s.Teachings.Any(t => t.AcademicEmpNr == teacherId))
                .Include(s => s.Department)
                .Include(s => s.Teachings)
                .OrderBy(s => s.Title)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subjects by teacher ID {TeacherId}", teacherId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Subject?> GetWithTeachingAssignmentsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting subject with teaching assignments by ID {Id}", id);
            return await _dbSet
                .Include(s => s.Department)
                .Include(s => s.Teachings)
                    .ThenInclude(t => t.Academic)
                .FirstOrDefaultAsync(s => s.Code == id.ToString(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subject with teaching assignments by ID {Id}", id);
            throw;
        }
    }
}