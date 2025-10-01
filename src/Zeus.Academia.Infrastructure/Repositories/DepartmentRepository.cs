using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Department entities with specialized operations.
/// </summary>
public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    /// <summary>
    /// Initializes a new instance of the DepartmentRepository class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    public DepartmentRepository(AcademiaDbContext context, ILogger<DepartmentRepository> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Department>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting departments by university ID {UniversityId}", universityId);
            // Note: Department schema doesn't have direct UniversityId relationship, returning all departments
            return await _dbSet
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments by university ID {UniversityId}", universityId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Department?> GetByCodeAsync(string deptCode, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(deptCode);
            _logger.LogDebug("Getting department by code {DeptCode}", deptCode);

            // Department uses Name as primary key, treating deptCode as Name
            return await _dbSet
                .FirstOrDefaultAsync(d => d.Name == deptCode, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department by code {DeptCode}", deptCode);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Department>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(namePattern);
            _logger.LogDebug("Searching departments by name pattern {NamePattern}", namePattern);

            var pattern = $"%{namePattern}%";
            return await _dbSet
                .Where(d => EF.Functions.Like(d.Name, pattern) || EF.Functions.Like(d.FullName, pattern))
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching departments by name pattern {NamePattern}", namePattern);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Department>> GetWithAcademicsCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting departments with academics count");
            return await _dbSet
                .Include(d => d.Professors)
                .Include(d => d.Teachers)
                .Include(d => d.Students)
                .Include(d => d.TeachingProfs)
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments with academics count");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Department?> GetWithAcademicsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting department with academics by ID {Id}", id);
            // Department uses string key, need to find by Name or use BaseEntity Id if it exists
            var department = await _dbSet.FindAsync(id);
            if (department == null) return null;

            return await _dbSet
                .Include(d => d.Professors)
                .Include(d => d.Teachers)
                .Include(d => d.Students)
                .Include(d => d.TeachingProfs)
                .FirstOrDefaultAsync(d => d.Name == department.Name, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department with academics by ID {Id}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Department?> GetWithSubjectsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting department with subjects by ID {Id}", id);
            var department = await _dbSet.FindAsync(id);
            if (department == null) return null;

            return await _dbSet
                .Include(d => d.Subjects)
                .FirstOrDefaultAsync(d => d.Name == department.Name, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department with subjects by ID {Id}", id);
            throw;
        }
    }
}