using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for user management operations.
/// Provides specialized user queries and operations for Identity users.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<UserRepository> _logger;
    private readonly DbSet<AcademiaUser> _dbSet;

    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    public UserRepository(AcademiaDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
        _dbSet = context.Set<AcademiaUser>();
    }

    #region Basic CRUD Operations

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all users");
            return await _dbSet.Where(u => u.IsActive).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<AcademiaUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user by ID: {UserId}", id);
            return await _dbSet.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<AcademiaUser> AddAsync(AcademiaUser entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Adding user: {UserName}", entity.UserName);
            entity.CreatedDate = DateTime.UtcNow;
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user: {UserName}", entity.UserName);
            throw;
        }
    }

    /// <inheritdoc/>
    public Task<AcademiaUser> UpdateAsync(AcademiaUser entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating user: {UserId}", entity.Id);
            entity.ModifiedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
            return Task.FromResult(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", entity.Id);
            throw;
        }
    }

    /// <inheritdoc/>
    public Task<bool> RemoveAsync(AcademiaUser entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Removing user: {UserId}", entity.Id);
            _dbSet.Remove(entity);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user: {UserId}", entity.Id);
            return Task.FromResult(false);
        }
    }

    #endregion

    #region User Queries

    /// <inheritdoc/>
    public async Task<AcademiaUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user by email: {Email}", email);
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<AcademiaUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user by username: {UserName}", userName);
            return await _dbSet
                .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by username: {UserName}", userName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<AcademiaUser?> GetWithAcademicAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user with academic info: {UserId}", userId);
            return await _dbSet
                .Include(u => u.Academic)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with academic info: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<AcademiaUser?> GetWithRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user with roles: {UserId}", userId);
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with roles: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<AcademiaUser?> GetWithRefreshTokensAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user with refresh tokens: {UserId}", userId);
            return await _dbSet
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with refresh tokens: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<AcademiaUser?> GetWithAllDataAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user with all related data: {UserId}", userId);
            return await _dbSet
                .Include(u => u.Academic)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with all related data: {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region User Search and Filtering

    /// <inheritdoc/>
    public async Task<(IEnumerable<AcademiaUser> Users, int TotalCount)> SearchUsersAsync(
        string? searchTerm = null,
        string? email = null,
        bool? isActive = null,
        bool? emailConfirmed = null,
        string? role = null,
        string? department = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        DateTime? lastLoginAfter = null,
        DateTime? lastLoginBefore = null,
        Expression<Func<AcademiaUser, object>>? orderBy = null,
        bool orderDescending = false,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Searching users with criteria - SearchTerm: {SearchTerm}, Email: {Email}, IsActive: {IsActive}, Page: {Page}, Size: {Size}",
                searchTerm, email, isActive, pageNumber, pageSize);

            var query = _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(u =>
                    u.UserName!.ToLower().Contains(lowerSearchTerm) ||
                    u.Email!.ToLower().Contains(lowerSearchTerm) ||
                    u.FirstName!.ToLower().Contains(lowerSearchTerm) ||
                    u.LastName!.ToLower().Contains(lowerSearchTerm) ||
                    u.DisplayName!.ToLower().Contains(lowerSearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(u => u.Email!.ToLower().Contains(email.ToLower()));
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            if (emailConfirmed.HasValue)
            {
                query = query.Where(u => u.EmailConfirmed == emailConfirmed.Value);
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => u.UserRoles.Any(ur =>
                    ur.Role.Name == role && ur.IsCurrentlyEffective()));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(u => u.UserRoles.Any(ur =>
                    ur.DepartmentContextName == department && ur.IsCurrentlyEffective()));
            }

            if (createdAfter.HasValue)
            {
                query = query.Where(u => u.CreatedDate >= createdAfter.Value);
            }

            if (createdBefore.HasValue)
            {
                query = query.Where(u => u.CreatedDate <= createdBefore.Value);
            }

            if (lastLoginAfter.HasValue)
            {
                query = query.Where(u => u.LastLoginDate >= lastLoginAfter.Value);
            }

            if (lastLoginBefore.HasValue)
            {
                query = query.Where(u => u.LastLoginDate <= lastLoginBefore.Value);
            }

            // Apply ordering
            if (orderBy != null)
            {
                query = orderDescending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);
            }
            else
            {
                query = orderDescending
                    ? query.OrderByDescending(u => u.CreatedDate)
                    : query.OrderBy(u => u.CreatedDate);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (users, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaUser>> GetByRoleAsync(string roleName, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting users by role: {RoleName}, IncludeInactive: {IncludeInactive}", roleName, includeInactive);

            var query = _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur =>
                    ur.Role.Name == roleName && ur.IsCurrentlyEffective()));

            if (!includeInactive)
            {
                query = query.Where(u => u.IsActive);
            }

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by role: {RoleName}", roleName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaUser>> GetByAcademicTypeAsync(Type academicType, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting users by academic type: {AcademicType}, IncludeInactive: {IncludeInactive}", academicType.Name, includeInactive);

            var query = _dbSet
                .Include(u => u.Academic)
                .Where(u => u.Academic != null && u.Academic.GetType() == academicType);

            if (!includeInactive)
            {
                query = query.Where(u => u.IsActive);
            }

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by academic type: {AcademicType}", academicType.Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaUser>> GetUnconfirmedEmailUsersAsync(int? olderThanDays = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting unconfirmed email users, OlderThanDays: {OlderThanDays}", olderThanDays);

            var query = _dbSet.Where(u => !u.EmailConfirmed && u.IsActive);

            if (olderThanDays.HasValue)
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays.Value);
                query = query.Where(u => u.CreatedDate <= cutoffDate);
            }

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unconfirmed email users");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaUser>> GetLockedOutUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting locked out users");

            var now = DateTimeOffset.UtcNow;
            return await _dbSet
                .Where(u => u.LockoutEnd.HasValue && u.LockoutEnd > now)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting locked out users");
            throw;
        }
    }

    #endregion

    #region User Validation

    /// <inheritdoc/>
    public async Task<bool> ExistsByEmailAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if user exists by email: {Email}, ExcludeUserId: {ExcludeUserId}", email, excludeUserId);

            var query = _dbSet.Where(u => u.Email == email);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists by email: {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByUserNameAsync(string userName, int? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if user exists by username: {UserName}, ExcludeUserId: {ExcludeUserId}", userName, excludeUserId);

            var query = _dbSet.Where(u => u.UserName == userName);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists by username: {UserName}", userName);
            throw;
        }
    }

    #endregion

    #region User Statistics

    /// <inheritdoc/>
    public async Task<UserStatistics> GetUserStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user statistics");

            var now = DateTime.UtcNow;
            var today = now.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var stats = new UserStatistics
            {
                TotalUsers = await _dbSet.CountAsync(cancellationToken),
                ActiveUsers = await _dbSet.CountAsync(u => u.IsActive, cancellationToken),
                InactiveUsers = await _dbSet.CountAsync(u => !u.IsActive, cancellationToken),
                ConfirmedEmailUsers = await _dbSet.CountAsync(u => u.EmailConfirmed, cancellationToken),
                UnconfirmedEmailUsers = await _dbSet.CountAsync(u => !u.EmailConfirmed, cancellationToken),
                LockedOutUsers = await _dbSet.CountAsync(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow, cancellationToken),
                UsersRegisteredToday = await _dbSet.CountAsync(u => u.CreatedDate >= today, cancellationToken),
                UsersRegisteredThisWeek = await _dbSet.CountAsync(u => u.CreatedDate >= weekStart, cancellationToken),
                UsersRegisteredThisMonth = await _dbSet.CountAsync(u => u.CreatedDate >= monthStart, cancellationToken)
            };

            // Get user counts by role
            var roleStats = await _context.UserRoles
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { UserRole = ur, Role = r })
                .GroupBy(x => x.Role.Name)
                .Select(g => new { RoleName = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            foreach (var roleStat in roleStats.Where(rs => rs.RoleName != null))
            {
                stats.UsersByRole[roleStat.RoleName!] = roleStat.Count;
            }

            // Get user counts by academic type
            var academicTypeStats = await _dbSet
                .Include(u => u.Academic)
                .Where(u => u.Academic != null)
                .GroupBy(u => u.Academic!.GetType().Name)
                .Select(g => new { TypeName = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            foreach (var typeStat in academicTypeStats)
            {
                stats.UsersByAcademicType[typeStat.TypeName] = typeStat.Count;
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user statistics");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<int> GetActiveUserCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting active user count");
            return await _dbSet.CountAsync(u => u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active user count");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaUser>> GetNewRegistrationsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting new registrations from {StartDate} to {EndDate}", startDate, endDate);

            return await _dbSet
                .Where(u => u.CreatedDate >= startDate && u.CreatedDate <= endDate)
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting new registrations from {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    #endregion
}