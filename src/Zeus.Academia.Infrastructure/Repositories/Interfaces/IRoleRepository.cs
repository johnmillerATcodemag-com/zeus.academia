using Zeus.Academia.Infrastructure.Identity;

namespace Zeus.Academia.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for role management operations.
/// Extends the generic repository with role-specific functionality.
/// </summary>
public interface IRoleRepository : IRepository<AcademiaRole>
{
    #region Role Queries

    /// <summary>
    /// Gets a role by name asynchronously.
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The role if found, otherwise null</returns>
    Task<AcademiaRole?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles with their user assignments asynchronously.
    /// </summary>
    /// <param name="includeInactiveUsers">Whether to include inactive users</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Roles with user assignments</returns>
    Task<IEnumerable<AcademiaRole>> GetWithUserAssignmentsAsync(bool includeInactiveUsers = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role with its user assignments asynchronously.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="includeInactiveUsers">Whether to include inactive users</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The role with user assignments if found, otherwise null</returns>
    Task<AcademiaRole?> GetWithUserAssignmentsAsync(int roleId, bool includeInactiveUsers = false, CancellationToken cancellationToken = default);

    #endregion

    #region Role Hierarchy

    /// <summary>
    /// Gets roles in hierarchical order asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Roles ordered by hierarchy</returns>
    Task<IEnumerable<AcademiaRole>> GetHierarchicalRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subordinate roles for a given role asynchronously.
    /// </summary>
    /// <param name="roleId">The parent role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Subordinate roles</returns>
    Task<IEnumerable<AcademiaRole>> GetSubordinateRolesAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles that a user can manage based on their role hierarchy asynchronously.
    /// </summary>
    /// <param name="userRoleId">The user's role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Manageable roles</returns>
    Task<IEnumerable<AcademiaRole>> GetManageableRolesAsync(int userRoleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if one role has authority over another role asynchronously.
    /// </summary>
    /// <param name="managerRoleId">The manager role ID</param>
    /// <param name="subordinateRoleId">The subordinate role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if manager role has authority, otherwise false</returns>
    Task<bool> HasAuthorityOverAsync(int managerRoleId, int subordinateRoleId, CancellationToken cancellationToken = default);

    #endregion

    #region Role Search and Filtering

    /// <summary>
    /// Searches roles by name or description asynchronously.
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="isActive">Active status filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching roles</returns>
    Task<IEnumerable<AcademiaRole>> SearchRolesAsync(string searchTerm, bool? isActive = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles by type asynchronously.
    /// </summary>
    /// <param name="roleType">The role type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Roles of the specified type</returns>
    Task<IEnumerable<AcademiaRole>> GetByTypeAsync(AcademicRoleType roleType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles by priority level asynchronously.
    /// </summary>
    /// <param name="priorityLevel">The priority level</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Roles at the specified priority level</returns>
    Task<IEnumerable<AcademiaRole>> GetByPriorityLevelAsync(int priorityLevel, CancellationToken cancellationToken = default);

    #endregion

    #region Role Validation

    /// <summary>
    /// Checks if a role exists by name asynchronously.
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="excludeRoleId">Optional role ID to exclude from check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if role exists, otherwise false</returns>
    Task<bool> ExistsByNameAsync(string roleName, int? excludeRoleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role has active user assignments asynchronously.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if role has active assignments, otherwise false</returns>
    Task<bool> HasActiveAssignmentsAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a role can be safely deleted asynchronously.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with any issues</returns>
    Task<RoleDeletionValidationResult> ValidateForDeletionAsync(int roleId, CancellationToken cancellationToken = default);

    #endregion

    #region Role Statistics

    /// <summary>
    /// Gets role statistics asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role statistics</returns>
    Task<RoleStatistics> GetRoleStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user count for each role asynchronously.
    /// </summary>
    /// <param name="includeInactiveUsers">Whether to include inactive users</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role user counts</returns>
    Task<Dictionary<string, int>> GetRoleUserCountsAsync(bool includeInactiveUsers = false, CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// Role deletion validation result.
/// </summary>
public class RoleDeletionValidationResult
{
    public bool CanDelete { get; set; }
    public List<string> Issues { get; set; } = new();
    public int ActiveAssignmentCount { get; set; }
    public int TotalAssignmentCount { get; set; }
}

/// <summary>
/// Role statistics data transfer object.
/// </summary>
public class RoleStatistics
{
    public int TotalRoles { get; set; }
    public int ActiveRoles { get; set; }
    public int InactiveRoles { get; set; }
    public Dictionary<AcademicRoleType, int> RolesByType { get; set; } = new();
    public Dictionary<int, int> RolesByPriorityLevel { get; set; } = new();
    public Dictionary<string, int> RoleUserCounts { get; set; } = new();
    public int RolesWithNoAssignments { get; set; }
    public int RolesWithActiveAssignments { get; set; }
}