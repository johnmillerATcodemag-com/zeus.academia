using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Data.Repositories;

/// <summary>
/// Academic repository implementation with domain-specific operations.
/// Task 6: Repository Pattern Implementation - Academic repository with business logic.
/// </summary>
public class AcademicRepository : Repository<Academic>, IAcademicRepository
{
    public AcademicRepository(AcademiaDbContext context, ILogger<Repository<Academic>> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc />
    public async Task<Academic?> GetByEmployeeNumberAsync(string employeeNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(employeeNumber))
                throw new ArgumentException("Employee number cannot be null or empty", nameof(employeeNumber));

            _logger.LogDebug("Getting academic by employee number {EmployeeNumber}", employeeNumber);
            return await _dbSet.FirstOrDefaultAsync(a => a.EmpNr.ToString() == employeeNumber, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting academic by employee number {EmployeeNumber}", employeeNumber);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Academic>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting academics by department {DepartmentId}", departmentId);
            // Note: Academic entity doesn't have direct department relationship in base class
            // This would need to be implemented differently based on the specific academic type
            var professors = await _context.Professors
                .Where(p => p.DepartmentName == departmentId.ToString())
                .Include(p => p.Department)
                .Include(p => p.Rank)
                .Cast<Academic>()
                .ToListAsync(cancellationToken);

            return professors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting academics by department {DepartmentId}", departmentId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Academic>> GetByRankAsync(int rankId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting academics by rank {RankId}", rankId);
            // Note: Academic entity doesn't have direct rank relationship in base class
            // This would need to be implemented differently based on the specific academic type
            var professors = await _context.Professors
                .Where(p => p.RankCode == rankId.ToString())
                .Include(p => p.Department)
                .Include(p => p.Rank)
                .Cast<Academic>()
                .ToListAsync(cancellationToken);

            return professors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting academics by rank {RankId}", rankId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Academic>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(namePattern))
                throw new ArgumentException("Name pattern cannot be null or empty", nameof(namePattern));

            _logger.LogDebug("Searching academics by name pattern {NamePattern}", namePattern);
            return await _dbSet
                .Where(a => EF.Functions.Like(a.Name, $"%{namePattern}%"))
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching academics by name pattern {NamePattern}", namePattern);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Academic>> GetWithDepartmentAndRankAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting academics with department and rank information");
            return await _dbSet
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting academics with department and rank information");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Academic>> GetProfessorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting professors");
            return await _context.Set<Professor>()
                .Include(p => p.Department)
                .Include(p => p.Rank)
                .OrderBy(p => p.Name)
                .Cast<Academic>()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting professors");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Academic>> GetTeachingProfessorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting teaching professors");
            return await _context.Set<TeachingProf>()
                .Include(tp => tp.Department)
                .Include(tp => tp.Rank)
                .OrderBy(tp => tp.Name)
                .Cast<Academic>()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting teaching professors");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Academic>> GetChairsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting department chairs");
            return await _context.Set<Chair>()
                .Include(c => c.Academic)
                .Include(c => c.Department)
                .Where(c => c.Academic != null)
                .Select(c => c.Academic!)
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department chairs");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsEmployeeNumberAvailableAsync(string employeeNumber, int? excludeEmpNr = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(employeeNumber))
                throw new ArgumentException("Employee number cannot be null or empty", nameof(employeeNumber));

            _logger.LogDebug("Checking if employee number {EmployeeNumber} is available", employeeNumber);

            var query = _dbSet.Where(a => a.EmpNr.ToString() == employeeNumber);
            if (excludeEmpNr.HasValue)
            {
                query = query.Where(a => a.EmpNr != excludeEmpNr.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if employee number {EmployeeNumber} is available", employeeNumber);
            throw;
        }
    }
}