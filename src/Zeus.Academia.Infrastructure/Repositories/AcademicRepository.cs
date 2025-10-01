using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Academic entities with specialized operations.
/// </summary>
public class AcademicRepository : Repository<Academic>, IAcademicRepository
{
    /// <summary>
    /// Initializes a new instance of the AcademicRepository class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    public AcademicRepository(AcademiaDbContext context, ILogger<AcademicRepository> logger)
        : base(context, logger)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Academic>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting academics by department name {DepartmentName}", departmentName);

            // Since Department uses Name as key, find department first
            var department = await _context.Departments.FindAsync(departmentName);
            if (department == null)
            {
                return new List<Academic>();
            }

            // Query concrete types by department name
            var professors = await _context.Professors.Where(p => p.DepartmentName == department.Name).ToListAsync(cancellationToken);
            var teachers = await _context.Teachers.Where(t => t.DepartmentName == department.Name).ToListAsync(cancellationToken);
            var students = await _context.Students.Where(s => s.DepartmentName == department.Name).ToListAsync(cancellationToken);
            var teachingProfs = await _context.TeachingProfs.Where(tp => tp.DepartmentName == department.Name).ToListAsync(cancellationToken);

            var result = new List<Academic>();
            result.AddRange(professors);
            result.AddRange(teachers);
            result.AddRange(students);
            result.AddRange(teachingProfs);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting academics by department name {DepartmentName}", departmentName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Academic?> GetByEmployeeNumberAsync(string empNr, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(empNr);
            if (!int.TryParse(empNr, out int empNumber))
            {
                _logger.LogWarning("Invalid employee number format: {EmployeeNumber}", empNr);
                return null;
            }

            _logger.LogDebug("Getting academic by employee number {EmployeeNumber}", empNr);

            return await _dbSet
                .FirstOrDefaultAsync(a => a.EmpNr == empNumber, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting academic by employee number {EmployeeNumber}", empNr);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Academic>> SearchByNameAsync(string namePattern, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(namePattern);
            _logger.LogDebug("Searching academics by name pattern {NamePattern}", namePattern);

            var pattern = $"%{namePattern}%";
            return await _dbSet
                .Where(a => EF.Functions.Like(a.Name, pattern))
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching academics by name pattern {NamePattern}", namePattern);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Academic>> GetByRankAsync(int rankId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting academics by rank ID {RankId}", rankId);
            // Note: This method should probably take string rankCode instead of int rankId
            // For now, we'll find the rank by the BaseEntity Id and then use its Code
            var rank = await _context.Ranks.FirstOrDefaultAsync(r => r.Id == rankId, cancellationToken);
            if (rank == null)
            {
                return new List<Academic>();
            }

            // Query concrete types that have rank relationships
            var professors = await _context.Professors.Where(p => p.RankCode == rank.Code).ToListAsync(cancellationToken);
            var teachingProfs = await _context.TeachingProfs.Where(tp => tp.RankCode == rank.Code).ToListAsync(cancellationToken);

            var result = new List<Academic>();
            result.AddRange(professors);
            result.AddRange(teachingProfs);

            return result.OrderBy(a => a.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting academics by rank ID {RankId}", rankId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Professor>> GetProfessorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all professors");
            return await _context.Professors
                .Include(p => p.Department)
                .Include(p => p.Rank)
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting professors");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Teacher>> GetTeachersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all teachers");
            return await _context.Teachers
                .Include(t => t.Department)
                .OrderBy(t => t.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting teachers");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Student>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all students");
            return await _context.Students
                .Include(s => s.Department)
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting students");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Academic>> GetChairsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all department chairs");
            var chairAcademics = await _context.Chairs
                .Where(c => c.IsActive && c.AcademicEmpNr.HasValue)
                .Include(c => c.Academic)
                .Include(c => c.Department)
                .Select(c => c.Academic!)
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken);

            return chairAcademics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department chairs");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TeachingProf>> GetTeachingProfessorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all teaching professors");
            return await _context.TeachingProfs
                .Include(tp => tp.Department)
                .OrderBy(tp => tp.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting teaching professors");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsEmployeeNumberAvailableAsync(string employeeNumber, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new ArgumentException("Employee number cannot be null or empty", nameof(employeeNumber));

        if (!int.TryParse(employeeNumber, out int empNr))
            throw new ArgumentException("Employee number must be a valid integer", nameof(employeeNumber));

        try
        {
            _logger.LogDebug("Checking availability of employee number {EmployeeNumber}", employeeNumber);

            var query = _context.Academics.Where(a => a.EmpNr == empNr);

            if (excludeId.HasValue)
            {
                query = query.Where(a => a.EmpNr != excludeId.Value);
            }

            var exists = await query.AnyAsync(cancellationToken);
            return !exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking employee number availability for {EmployeeNumber}", employeeNumber);
            throw;
        }
    }
}