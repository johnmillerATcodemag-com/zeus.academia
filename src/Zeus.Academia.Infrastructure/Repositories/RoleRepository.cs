using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for role management operations.
/// Provides specialized role queries and hierarchy operations for AcademiaRole entities.
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<RoleRepository> _logger;
    private readonly DbSet<AcademiaRole> _dbSet;

    /// <summary>
    /// Initializes a new instance of the RoleRepository class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    public RoleRepository(AcademiaDbContext context, ILogger<RoleRepository> logger)
    {
        _context = context;
        _logger = logger;
        _dbSet = context.Set<AcademiaRole>();
    }

    #region Basic CRUD Operations

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaRole>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all roles");
            return await _dbSet.Where(r => r.IsActive).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<AcademiaRole?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting role by ID: {RoleId}", id);
            return await _dbSet.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role by ID: {RoleId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<AcademiaRole> AddAsync(AcademiaRole entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Adding role: {RoleName}", entity.Name);
            entity.CreatedDate = DateTime.UtcNow;
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding role: {RoleName}", entity.Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public Task<AcademiaRole> UpdateAsync(AcademiaRole entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating role: {RoleId}", entity.Id);
            entity.ModifiedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
            return Task.FromResult(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role: {RoleId}", entity.Id);
            throw;
        }
    }

    /// <inheritdoc/>
    public Task<bool> RemoveAsync(AcademiaRole entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Removing role: {RoleId}", entity.Id);
            _dbSet.Remove(entity);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role: {RoleId}", entity.Id);
            return Task.FromResult(false);
        }
    }

    #endregion

    #region Role Queries

    /// <inheritdoc/>
    public async Task<AcademiaRole?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting role by name: {RoleName}", roleName);
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role by name: {RoleName}", roleName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaRole>> GetWithUserAssignmentsAsync(bool includeInactiveUsers = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting roles with user assignments, IncludeInactiveUsers: {IncludeInactiveUsers}", includeInactiveUsers);

            var query = _dbSet
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .AsQueryable();

            if (!includeInactiveUsers)
            {
                query = query.Where(r => r.UserRoles.Any(ur => ur.User.IsActive && ur.IsCurrentlyEffective()));
            }

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles with user assignments");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<AcademiaRole?> GetWithUserAssignmentsAsync(int roleId, bool includeInactiveUsers = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting role with user assignments: {RoleId}, IncludeInactiveUsers: {IncludeInactiveUsers}", roleId, includeInactiveUsers);

            var query = _dbSet
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .Where(r => r.Id == roleId);

            var role = await query.FirstOrDefaultAsync(cancellationToken);

            if (role != null && !includeInactiveUsers)
            {
                // Filter out inactive users from the loaded role
                role.UserRoles = role.UserRoles
                    .Where(ur => ur.User.IsActive && ur.IsCurrentlyEffective())
                    .ToList();
            }

            return role;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role with user assignments: {RoleId}", roleId);
            throw;
        }
    }

    #endregion

    #region Role Hierarchy

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaRole>> GetHierarchicalRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting roles in hierarchical order");
            return await _dbSet
                .OrderBy(r => r.Priority)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hierarchical roles");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaRole>> GetSubordinateRolesAsync(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting subordinate roles for role: {RoleId}", roleId);

            var parentRole = await _dbSet.FindAsync(new object[] { roleId }, cancellationToken);
            if (parentRole == null)
            {
                return Enumerable.Empty<AcademiaRole>();
            }

            return await _dbSet
                .Where(r => r.Priority > parentRole.Priority)
                .OrderBy(r => r.Priority)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subordinate roles for role: {RoleId}", roleId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaRole>> GetManageableRolesAsync(int userRoleId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting manageable roles for user role: {UserRoleId}", userRoleId);

            var userRole = await _dbSet.FindAsync(new object[] { userRoleId }, cancellationToken);
            if (userRole == null)
            {
                return Enumerable.Empty<AcademiaRole>();
            }

            // User can manage roles at their level or below, excluding themselves
            return await _dbSet
                .Where(r => r.Priority >= userRole.Priority && r.Id != userRoleId)
                .OrderBy(r => r.Priority)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manageable roles for user role: {UserRoleId}", userRoleId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HasAuthorityOverAsync(int managerRoleId, int subordinateRoleId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking authority: Manager Role {ManagerRoleId} over Subordinate Role {SubordinateRoleId}", managerRoleId, subordinateRoleId);

            var roles = await _dbSet
                .Where(r => r.Id == managerRoleId || r.Id == subordinateRoleId)
                .ToListAsync(cancellationToken);

            var managerRole = roles.FirstOrDefault(r => r.Id == managerRoleId);
            var subordinateRole = roles.FirstOrDefault(r => r.Id == subordinateRoleId);

            if (managerRole == null || subordinateRole == null)
            {
                return false;
            }

            // Lower hierarchy level means higher authority
            return managerRole.Priority > subordinateRole.Priority;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking authority between roles: {ManagerRoleId} and {SubordinateRoleId}", managerRoleId, subordinateRoleId);
            throw;
        }
    }

    #endregion

    #region Role Search and Filtering

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaRole>> SearchRolesAsync(string searchTerm, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Searching roles with term: {SearchTerm}, IsActive: {IsActive}", searchTerm, isActive);

            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(r =>
                    (r.Name != null && r.Name.ToLower().Contains(lowerSearchTerm)) ||
                    (r.Description != null && r.Description.ToLower().Contains(lowerSearchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(r => r.IsActive == isActive.Value);
            }

            return await query
                .OrderBy(r => r.Priority)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching roles with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaRole>> GetByTypeAsync(AcademicRoleType roleType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting roles by type: {RoleType}", roleType);
            return await _dbSet
                .Where(r => r.RoleType == roleType)
                .OrderBy(r => r.Priority)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles by type: {RoleType}", roleType);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AcademiaRole>> GetByPriorityLevelAsync(int priorityLevel, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting roles by priority level: {PriorityLevel}", priorityLevel);
            return await _dbSet
                .Where(r => r.Priority == priorityLevel)
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles by priority level: {PriorityLevel}", priorityLevel);
            throw;
        }
    }

    #endregion

    #region Role Validation

    /// <inheritdoc/>
    public async Task<bool> ExistsByNameAsync(string roleName, int? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if role exists by name: {RoleName}, ExcludeRoleId: {ExcludeRoleId}", roleName, excludeRoleId);

            var query = _dbSet.Where(r => r.Name == roleName);

            if (excludeRoleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRoleId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if role exists by name: {RoleName}", roleName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HasActiveAssignmentsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if role has active assignments: {RoleId}", roleId);

            return await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Join(_context.Users, ur => ur.UserId, u => u.Id, (ur, u) => u)
                .AnyAsync(u => u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if role has active assignments: {RoleId}", roleId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<RoleDeletionValidationResult> ValidateForDeletionAsync(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Validating role for deletion: {RoleId}", roleId);

            var result = new RoleDeletionValidationResult();

            var role = await _dbSet.FindAsync(new object[] { roleId }, cancellationToken);
            if (role == null)
            {
                result.Issues.Add("Role not found");
                return result;
            }

            // Check for active assignments
            var activeAssignments = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Join(_context.Users, ur => ur.UserId, u => u.Id, (ur, u) => u)
                .CountAsync(u => u.IsActive, cancellationToken);

            var totalAssignments = await _context.UserRoles
                .CountAsync(ur => ur.RoleId == roleId, cancellationToken);

            result.ActiveAssignmentCount = activeAssignments;
            result.TotalAssignmentCount = totalAssignments;

            if (activeAssignments > 0)
            {
                result.Issues.Add($"Role has {activeAssignments} active user assignments");
            }

            // Check if it's a system role that shouldn't be deleted
            if (role.Name != null && IsSystemRole(role.Name))
            {
                result.Issues.Add("System roles cannot be deleted");
            }

            result.CanDelete = result.Issues.Count == 0;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating role for deletion: {RoleId}", roleId);
            throw;
        }
    }

    #endregion

    #region Role Statistics

    /// <inheritdoc/>
    public async Task<RoleStatistics> GetRoleStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting role statistics");

            var stats = new RoleStatistics
            {
                TotalRoles = await _dbSet.CountAsync(cancellationToken),
                ActiveRoles = await _dbSet.CountAsync(r => r.IsActive, cancellationToken),
                InactiveRoles = await _dbSet.CountAsync(r => !r.IsActive, cancellationToken)
            };

            // Get roles by type
            var rolesByType = await _dbSet
                .GroupBy(r => r.RoleType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            foreach (var typeGroup in rolesByType)
            {
                stats.RolesByType[typeGroup.Type] = typeGroup.Count;
            }

            // Get roles by hierarchy level
            var rolesByHierarchy = await _dbSet
                .GroupBy(r => r.Priority)
                .Select(g => new { Level = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            foreach (var hierarchyGroup in rolesByHierarchy)
            {
                stats.RolesByPriorityLevel[hierarchyGroup.Level] = hierarchyGroup.Count;
            }

            // Get role user counts
            stats.RoleUserCounts = await GetRoleUserCountsAsync(false, cancellationToken);

            // Count roles with no assignments and with active assignments
            stats.RolesWithNoAssignments = await _dbSet
                .CountAsync(r => !r.UserRoles.Any(), cancellationToken);

            stats.RolesWithActiveAssignments = await _dbSet
                .CountAsync(r => r.UserRoles.Any(ur => ur.IsCurrentlyEffective() && ur.User.IsActive), cancellationToken);

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role statistics");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, int>> GetRoleUserCountsAsync(bool includeInactiveUsers = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting role user counts, IncludeInactiveUsers: {IncludeInactiveUsers}", includeInactiveUsers);

            var query = _context.UserRoles
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { UserRole = ur, Role = r })
                .Join(_context.Users, combined => combined.UserRole.UserId, u => u.Id, (combined, u) => new { combined.UserRole, combined.Role, User = u });

            if (!includeInactiveUsers)
            {
                query = query.Where(x => x.User.IsActive);
            }

            var roleCounts = await query
                .GroupBy(x => x.Role.Name)
                .Select(g => new { RoleName = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return roleCounts
                .Where(rc => rc.RoleName != null)
                .ToDictionary(rc => rc.RoleName!, rc => rc.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role user counts");
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Checks if a role is a system role that shouldn't be deleted.
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <returns>True if it's a system role</returns>
    private static bool IsSystemRole(string roleName)
    {
        var systemRoles = new[] { "Administrator", "SuperAdmin", "SystemUser" };
        return systemRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }

    #endregion
}