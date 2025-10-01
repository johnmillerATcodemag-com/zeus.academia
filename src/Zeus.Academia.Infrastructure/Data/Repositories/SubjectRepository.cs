using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Data.Repositories;

/// <summary>
/// Subject repository implementation with domain-specific operations.
/// Task 6: Repository Pattern Implementation - Subject repository with business logic.
/// </summary>
public class SubjectRepository : Repository<Subject>, ISubjectRepository
{
    public SubjectRepository(AcademiaDbContext context, ILogger<Repository<Subject>> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc />
    public async Task<Subject?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Subject code cannot be null or empty", nameof(code));

            _logger.LogDebug("Getting subject by code {Code}", code);
            return await _dbSet.FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subject by code {Code}", code);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Subject>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting subjects by department {DepartmentId}", departmentId);
            return await _dbSet
                .Where(s => s.DepartmentName == departmentId.ToString())
                .Include(s => s.Department)
                .OrderBy(s => s.Title)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subjects by department {DepartmentId}", departmentId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Subject>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(namePattern))
                throw new ArgumentException("Name pattern cannot be null or empty", nameof(namePattern));

            _logger.LogDebug("Searching subjects by name pattern {NamePattern}", namePattern);
            return await _dbSet
                .Where(s => EF.Functions.Like(s.Title, $"%{namePattern}%"))
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

    /// <inheritdoc />
    public async Task<IEnumerable<Subject>> GetWithDepartmentAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting subjects with department information");
            return await _dbSet
                .Include(s => s.Department)
                .OrderBy(s => s.Title)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subjects with department information");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Subject>> GetActiveSubjectsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting active subjects");
            // Note: This assumes there might be an IsActive property or similar logic
            // For now, we'll return all subjects since there's no IsActive property in the current entity
            return await _dbSet
                .Include(s => s.Department)
                .OrderBy(s => s.Title)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active subjects");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsCodeAvailableAsync(string code, string? excludeCode = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Subject code cannot be null or empty", nameof(code));

            _logger.LogDebug("Checking if subject code {Code} is available", code);

            var query = _dbSet.Where(s => s.Code == code);
            if (!string.IsNullOrWhiteSpace(excludeCode))
            {
                query = query.Where(s => s.Code != excludeCode);
            }

            return !await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if subject code {Code} is available", code);
            throw;
        }
    }
}