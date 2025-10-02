using System.Text.Json;

namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Service for managing role hierarchy and permissions in the Zeus Academia System.
/// Provides functionality for role comparison, permission calculation, and access control.
/// </summary>
public interface IRoleHierarchyService
{
    /// <summary>
    /// Determines if one role can manage another role based on hierarchy.
    /// </summary>
    /// <param name="managerRole">The role attempting to manage</param>
    /// <param name="targetRole">The role being managed</param>
    /// <returns>True if the manager role can manage the target role</returns>
    bool CanManageRole(AcademiaRole managerRole, AcademiaRole targetRole);

    /// <summary>
    /// Gets the effective permissions for a role, including inherited permissions.
    /// </summary>
    /// <param name="role">The role to get permissions for</param>
    /// <returns>The combined permissions for the role</returns>
    AcademiaPermission GetEffectivePermissions(AcademiaRole role);

    /// <summary>
    /// Gets the effective permissions for a user across all their roles.
    /// </summary>
    /// <param name="user">The user to get permissions for</param>
    /// <returns>The combined permissions across all user roles</returns>
    AcademiaPermission GetUserPermissions(AcademiaUser user);

    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="permission">The permission to check for</param>
    /// <returns>True if the user has the permission</returns>
    bool UserHasPermission(AcademiaUser user, AcademiaPermission permission);

    /// <summary>
    /// Checks if a user has any of the specified permissions.
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="permissions">The permissions to check for</param>
    /// <returns>True if the user has any of the permissions</returns>
    bool UserHasAnyPermission(AcademiaUser user, params AcademiaPermission[] permissions);

    /// <summary>
    /// Gets the highest priority role for a user.
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>The highest priority role, or null if no roles</returns>
    AcademiaRole? GetUserHighestRole(AcademiaUser user);

    /// <summary>
    /// Creates a role with default permissions for the specified role type.
    /// </summary>
    /// <param name="roleType">The type of role to create</param>
    /// <param name="departmentName">Optional department name for department-specific roles</param>
    /// <param name="createdBy">Who is creating the role</param>
    /// <returns>A new role with default configuration</returns>
    AcademiaRole CreateRole(AcademicRoleType roleType, string? departmentName = null, string? createdBy = null);

    /// <summary>
    /// Gets all roles that a given role can manage.
    /// </summary>
    /// <param name="managerRole">The role to check management capabilities for</param>
    /// <param name="availableRoles">The available roles to filter</param>
    /// <returns>Roles that can be managed by the manager role</returns>
    IEnumerable<AcademiaRole> GetManageableRoles(AcademiaRole managerRole, IEnumerable<AcademiaRole> availableRoles);

    /// <summary>
    /// Checks if a user has the specified role or a higher priority role.
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="requiredRole">The minimum required role</param>
    /// <returns>True if the user has the required role or higher</returns>
    bool HasRoleOrHigher(AcademiaUser user, AcademicRoleType requiredRole);
}

/// <summary>
/// Implementation of role hierarchy service for the Zeus Academia System.
/// </summary>
public class RoleHierarchyService : IRoleHierarchyService
{
    /// <summary>
    /// Determines if one role can manage another role based on hierarchy.
    /// </summary>
    public bool CanManageRole(AcademiaRole managerRole, AcademiaRole targetRole)
    {
        // Inactive roles cannot manage anything
        if (!managerRole.IsActive || !targetRole.IsActive)
            return false;

        // System admins can manage everything
        if (managerRole.RoleType == AcademicRoleType.SystemAdmin)
            return true;

        // Users cannot manage roles with higher or equal priority (except system admin managing everything)
        if (managerRole.Priority <= targetRole.Priority)
            return false;

        // Department-specific management rules
        if (managerRole.RoleType == AcademicRoleType.Chair)
        {
            // Chairs can manage within their department or roles with lower priority
            if (!string.IsNullOrEmpty(managerRole.DepartmentName))
            {
                return (targetRole.DepartmentName == managerRole.DepartmentName) ||
                       (managerRole.Priority > targetRole.Priority);
            }
        }

        // General hierarchy: higher priority can manage lower priority
        return managerRole.Priority > targetRole.Priority;
    }

    /// <summary>
    /// Gets the effective permissions for a role, including inherited permissions.
    /// </summary>
    public AcademiaPermission GetEffectivePermissions(AcademiaRole role)
    {
        // Start with default permissions for the role type
        var permissions = role.RoleType.GetDefaultPermissions();

        // Add any additional permissions stored in the role
        if (!string.IsNullOrEmpty(role.AdditionalPermissions))
        {
            try
            {
                var additionalPerms = JsonSerializer.Deserialize<AcademiaPermission>(role.AdditionalPermissions);
                permissions |= additionalPerms;
            }
            catch (JsonException)
            {
                // If JSON parsing fails, just use default permissions
            }
        }

        // Inactive roles have no permissions
        if (!role.IsActive)
            return AcademiaPermission.None;

        return permissions;
    }

    /// <summary>
    /// Gets the effective permissions for a user across all their roles.
    /// </summary>
    public AcademiaPermission GetUserPermissions(AcademiaUser user)
    {
        if (!user.IsActive)
            return AcademiaPermission.None;

        var allPermissions = AcademiaPermission.None;

        // Combine permissions from all effective roles
        foreach (var userRole in user.GetEffectiveUserRoles())
        {
            var rolePermissions = GetEffectivePermissions(userRole.Role);
            allPermissions |= rolePermissions;
        }

        return allPermissions;
    }

    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    public bool UserHasPermission(AcademiaUser user, AcademiaPermission permission)
    {
        var userPermissions = GetUserPermissions(user);
        return userPermissions.HasPermission(permission);
    }

    /// <summary>
    /// Checks if a user has any of the specified permissions.
    /// </summary>
    public bool UserHasAnyPermission(AcademiaUser user, params AcademiaPermission[] permissions)
    {
        var userPermissions = GetUserPermissions(user);
        return userPermissions.HasAnyPermission(permissions);
    }

    /// <summary>
    /// Gets the highest priority role for a user.
    /// </summary>
    public AcademiaRole? GetUserHighestRole(AcademiaUser user)
    {
        return user.GetEffectiveRoles()
            .OrderByDescending(r => r.Priority)
            .FirstOrDefault();
    }

    /// <summary>
    /// Creates a role with default permissions for the specified role type.
    /// </summary>
    public AcademiaRole CreateRole(AcademicRoleType roleType, string? departmentName = null, string? createdBy = null)
    {
        var role = new AcademiaRole
        {
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            CreatedBy = createdBy ?? "System",
            ModifiedBy = createdBy ?? "System"
        };

        role.InitializeFromRoleType(roleType, departmentName);

        // Store default permissions as additional permissions for flexibility
        var defaultPermissions = roleType.GetDefaultPermissions();
        if (defaultPermissions != AcademiaPermission.None)
        {
            role.AdditionalPermissions = JsonSerializer.Serialize(defaultPermissions);
        }

        return role;
    }

    /// <summary>
    /// Gets all roles that a given role can manage.
    /// </summary>
    public IEnumerable<AcademiaRole> GetManageableRoles(AcademiaRole managerRole, IEnumerable<AcademiaRole> availableRoles)
    {
        return availableRoles.Where(role => CanManageRole(managerRole, role));
    }

    /// <summary>
    /// Validates role assignment based on business rules and hierarchy.
    /// </summary>
    /// <param name="assignerRole">The role of the person making the assignment</param>
    /// <param name="targetUser">The user receiving the role assignment</param>
    /// <param name="roleToAssign">The role being assigned</param>
    /// <returns>True if the assignment is valid</returns>
    public bool ValidateRoleAssignment(AcademiaRole assignerRole, AcademiaUser targetUser, AcademiaRole roleToAssign)
    {
        // Check if the assigner can manage the role being assigned
        if (!CanManageRole(assignerRole, roleToAssign))
            return false;

        // Check if the target user is active
        if (!targetUser.IsActive)
            return false;

        // Business rule: Students should have Academic records
        if (roleToAssign.RoleType == AcademicRoleType.Student && targetUser.Academic == null)
            return false;

        // Business rule: Professor/Chair roles should have Academic records
        if ((roleToAssign.RoleType == AcademicRoleType.Professor ||
             roleToAssign.RoleType == AcademicRoleType.Chair ||
             roleToAssign.RoleType == AcademicRoleType.TeachingProf) &&
            targetUser.Academic == null)
            return false;

        // Department-specific validation
        if (!string.IsNullOrEmpty(roleToAssign.DepartmentName) && targetUser.Academic != null)
        {
            // For department-specific roles, the user's academic record should be in the same department
            // This would require additional navigation properties to validate properly
            // For now, we'll allow it and rely on business logic elsewhere
        }

        return true;
    }

    /// <summary>
    /// Gets a human-readable description of the role hierarchy.
    /// </summary>
    /// <returns>Description of the role hierarchy</returns>
    public string GetHierarchyDescription()
    {
        var roles = Enum.GetValues<AcademicRoleType>()
            .OrderByDescending(r => r.GetPriority())
            .Select(r => $"{r.GetDisplayName()} (Priority: {r.GetPriority()})")
            .ToList();

        return string.Join(" > ", roles);
    }

    /// <summary>
    /// Checks if a user has the specified role or a higher priority role.
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="requiredRole">The minimum required role</param>
    /// <returns>True if the user has the required role or higher</returns>
    public bool HasRoleOrHigher(AcademiaUser user, AcademicRoleType requiredRole)
    {
        if (user == null || !user.IsActive)
            return false;

        var userRoles = user.GetEffectiveRoles();
        if (!userRoles.Any())
            return false;

        var requiredPriority = requiredRole.GetPriority();

        // Check if user has the exact role or a higher priority role
        return userRoles.Any(r => r.RoleType == requiredRole) ||
               userRoles.Any(r => r.Priority > requiredPriority);
    }
}