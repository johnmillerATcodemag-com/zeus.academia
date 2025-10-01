using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for role management operations.
/// Provides specialized role queries and hierarchy operations extending the base repository.
/// </summary>
public class RoleRepository : Repository<AcademiaRole>, IRoleRepository
{
    /// <summary>
    /// Initializes a new instance of the RoleRepository class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    public RoleRepository(AcademiaDbContext context, ILogger<RoleRepository> logger)
        : base(context, logger)
    {
    }

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
                    r.Name.ToLower().Contains(lowerSearchTerm) ||
                    r.Description.ToLower().Contains(lowerSearchTerm));
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
                .AnyAsync(ur => ur.RoleId == roleId && ur.IsCurrentlyEffective() && ur.User.IsActive, cancellationToken);
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
                .CountAsync(ur => ur.RoleId == roleId && ur.IsCurrentlyEffective() && ur.User.IsActive, cancellationToken);

            var totalAssignments = await _context.UserRoles
                .CountAsync(ur => ur.RoleId == roleId, cancellationToken);

            result.ActiveAssignmentCount = activeAssignments;
            result.TotalAssignmentCount = totalAssignments;

            if (activeAssignments > 0)
            {
                result.Issues.Add($"Role has {activeAssignments} active user assignments");
            }

            // Check if it's a system role that shouldn't be deleted
            if (IsSystemRole(role.Name))
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
                .Include(ur => ur.Role)
                .Where(ur => ur.IsCurrentlyEffective());

            if (!includeInactiveUsers)
            {
                query = query.Where(ur => ur.User.IsActive);
            }

            var roleCounts = await query
                .GroupBy(ur => ur.Role.Name)
                .Select(g => new { RoleName = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return roleCounts.ToDictionary(rc => rc.RoleName, rc => rc.Count);
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