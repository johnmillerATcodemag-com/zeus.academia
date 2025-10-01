using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Data.Repositories;

/// <summary>
/// Department repository implementation with domain-specific operations.
/// Task 6: Repository Pattern Implementation - Department repository with business logic.
/// </summary>
public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AcademiaDbContext context, ILogger<Repository<Department>> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc />
    public async Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Department code cannot be null or empty", nameof(code));

            _logger.LogDebug("Getting department by code {Code}", code);
            return await _dbSet.FirstOrDefaultAsync(d => d.Name == code, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department by code {Code}", code);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Department>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting departments by university {UniversityId}", universityId);
            // Department entity doesn't have UniversityId relationship in current schema
            // Returning all departments for now
            return await _dbSet
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments by university {UniversityId}", universityId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Department>> GetWithChairAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting departments with chair information");
            return await _dbSet
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments with chair information");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DepartmentWithCount>> GetWithAcademicCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting departments with academic counts");
            return await _dbSet
                .Select(d => new DepartmentWithCount
                {
                    Department = d,
                    AcademicCount = d.Professors.Count()
                })
                .OrderBy(dc => dc.Department.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments with academic counts");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Department>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(namePattern))
                throw new ArgumentException("Name pattern cannot be null or empty", nameof(namePattern));

            _logger.LogDebug("Searching departments by name pattern {NamePattern}", namePattern);
            return await _dbSet
                .Where(d => EF.Functions.Like(d.Name, $"%{namePattern}%"))
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching departments by name pattern {NamePattern}", namePattern);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsCodeAvailableAsync(string code, string? excludeName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Department code cannot be null or empty", nameof(code));

            _logger.LogDebug("Checking if department code {Code} is available", code);

            var query = _dbSet.Where(d => d.Name == code);
            if (!string.IsNullOrWhiteSpace(excludeName))
            {
                query = query.Where(d => d.Name != excludeName);
            }

            return !await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if department code {Code} is available", code);
            throw;
        }
    }
}