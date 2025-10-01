namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Base class for authorization handlers in the Zeus Academia System.
/// Provides common functionality for checking user roles and permissions.
/// </summary>
public abstract class AcademiaAuthorizationHandler<TRequirement>
    where TRequirement : IAcademiaAuthorizationRequirement
{
    protected readonly IRoleHierarchyService _roleHierarchyService;

    protected AcademiaAuthorizationHandler(IRoleHierarchyService roleHierarchyService)
    {
        _roleHierarchyService = roleHierarchyService;
    }

    /// <summary>
    /// Gets the AcademiaUser from the user principal if available.
    /// This method should be implemented by the API layer where ClaimsPrincipal is available.
    /// </summary>
    /// <param name="userPrincipal">The user principal (object to avoid dependency on ClaimsPrincipal)</param>
    /// <returns>The AcademiaUser if found, otherwise null</returns>
    protected abstract AcademiaUser? GetAcademiaUser(object userPrincipal);

    /// <summary>
    /// Handles the authorization requirement.
    /// </summary>
    /// <param name="requirement">The requirement to evaluate</param>
    /// <param name="userPrincipal">The user principal</param>
    /// <returns>True if authorization is granted</returns>
    public abstract bool HandleRequirement(TRequirement requirement, object userPrincipal);
}

/// <summary>
/// Handler for role-based authorization requirements.
/// </summary>
public class RoleRequirementHandler : AcademiaAuthorizationHandler<AcademiaRequirements.RoleRequirement>
{
    public RoleRequirementHandler(IRoleHierarchyService roleHierarchyService)
        : base(roleHierarchyService)
    {
    }

    protected override AcademiaUser? GetAcademiaUser(object userPrincipal)
    {
        // This will be implemented in the API layer where ClaimsPrincipal is available
        throw new NotImplementedException("This method should be overridden in the API layer");
    }

    public override bool HandleRequirement(AcademiaRequirements.RoleRequirement requirement, object userPrincipal)
    {
        var user = GetAcademiaUser(userPrincipal);
        if (user == null || !user.IsActive)
            return false;

        if (requirement.AllowHigherRoles)
        {
            // Check if user has the required role or a higher priority role
            var userRoles = user.GetEffectiveRoles();
            var hasRequiredRole = userRoles.Any(r => r.RoleType == requirement.RequiredRole);

            if (hasRequiredRole)
                return true;

            // Check for higher priority roles
            var requiredPriority = requirement.RequiredRole.GetPriority();
            return userRoles.Any(r => r.Priority > requiredPriority);
        }
        else
        {
            // Exact role match required
            return user.HasRole(requirement.RequiredRole);
        }
    }
}

/// <summary>
/// Handler for permission-based authorization requirements.
/// </summary>
public class PermissionRequirementHandler : AcademiaAuthorizationHandler<AcademiaRequirements.PermissionRequirement>
{
    public PermissionRequirementHandler(IRoleHierarchyService roleHierarchyService)
        : base(roleHierarchyService)
    {
    }

    protected override AcademiaUser? GetAcademiaUser(object userPrincipal)
    {
        throw new NotImplementedException("This method should be overridden in the API layer");
    }

    public override bool HandleRequirement(AcademiaRequirements.PermissionRequirement requirement, object userPrincipal)
    {
        var user = GetAcademiaUser(userPrincipal);
        if (user == null)
            return false;

        if (requirement.RequireAll)
        {
            // User must have all specified permissions
            return _roleHierarchyService.UserHasPermission(user, requirement.RequiredPermission);
        }
        else
        {
            // User must have any of the specified permissions
            var individualPermissions = requirement.RequiredPermission.GetIndividualPermissions().ToArray();
            return _roleHierarchyService.UserHasAnyPermission(user, individualPermissions);
        }
    }
}

/// <summary>
/// Handler for department-based authorization requirements.
/// </summary>
public class DepartmentRequirementHandler : AcademiaAuthorizationHandler<AcademiaRequirements.DepartmentRequirement>
{
    public DepartmentRequirementHandler(IRoleHierarchyService roleHierarchyService)
        : base(roleHierarchyService)
    {
    }

    protected override AcademiaUser? GetAcademiaUser(object userPrincipal)
    {
        throw new NotImplementedException("This method should be overridden in the API layer");
    }

    public override bool HandleRequirement(AcademiaRequirements.DepartmentRequirement requirement, object userPrincipal)
    {
        var user = GetAcademiaUser(userPrincipal);
        if (user == null)
            return false;

        // System admins can access any department if allowed
        if (requirement.AllowSystemAdmin && user.HasRole(AcademicRoleType.SystemAdmin))
            return true;

        // If no specific department is required, any department access is allowed
        if (string.IsNullOrEmpty(requirement.DepartmentName))
            return true;

        // Check if user has roles in the required department
        var userRoles = user.GetEffectiveUserRoles();
        return userRoles.Any(ur =>
            ur.Role.DepartmentName == requirement.DepartmentName ||
            ur.DepartmentContextName == requirement.DepartmentName);
    }
}

/// <summary>
/// Handler for resource ownership authorization requirements.
/// </summary>
public class ResourceOwnershipRequirementHandler : AcademiaAuthorizationHandler<AcademiaRequirements.ResourceOwnershipRequirement>
{
    public ResourceOwnershipRequirementHandler(IRoleHierarchyService roleHierarchyService)
        : base(roleHierarchyService)
    {
    }

    protected override AcademiaUser? GetAcademiaUser(object userPrincipal)
    {
        throw new NotImplementedException("This method should be overridden in the API layer");
    }

    public override bool HandleRequirement(AcademiaRequirements.ResourceOwnershipRequirement requirement, object userPrincipal)
    {
        var user = GetAcademiaUser(userPrincipal);
        if (user == null)
            return false;

        // Admin override
        if (requirement.AllowAdminOverride &&
            (user.HasRole(AcademicRoleType.SystemAdmin) || user.HasRole(AcademicRoleType.Administrator)))
            return true;

        // Department access - this would need to be implemented with specific resource context
        if (requirement.AllowDepartmentAccess)
        {
            // This would require additional context about the resource being accessed
            // For now, we'll allow chairs to access department resources
            return user.HasRole(AcademicRoleType.Chair);
        }

        // Resource ownership check would be implemented based on specific resource type
        // This is a placeholder that should be overridden for specific resources
        return false;
    }
}

/// <summary>
/// Handler for minimum role priority authorization requirements.
/// </summary>
public class MinimumRolePriorityRequirementHandler : AcademiaAuthorizationHandler<AcademiaRequirements.MinimumRolePriorityRequirement>
{
    public MinimumRolePriorityRequirementHandler(IRoleHierarchyService roleHierarchyService)
        : base(roleHierarchyService)
    {
    }

    protected override AcademiaUser? GetAcademiaUser(object userPrincipal)
    {
        throw new NotImplementedException("This method should be overridden in the API layer");
    }

    public override bool HandleRequirement(AcademiaRequirements.MinimumRolePriorityRequirement requirement, object userPrincipal)
    {
        var user = GetAcademiaUser(userPrincipal);
        if (user == null)
            return false;

        var highestRole = _roleHierarchyService.GetUserHighestRole(user);
        return highestRole != null && highestRole.Priority >= requirement.MinimumPriority;
    }
}

/// <summary>
/// Factory for creating authorization handlers.
/// </summary>
public static class AuthorizationHandlerFactory
{
    /// <summary>
    /// Creates all authorization handlers for the Zeus Academia System.
    /// </summary>
    /// <param name="roleHierarchyService">The role hierarchy service</param>
    /// <returns>Dictionary of requirement types to handlers</returns>
    public static Dictionary<Type, object> CreateHandlers(IRoleHierarchyService roleHierarchyService)
    {
        return new Dictionary<Type, object>
        {
            { typeof(AcademiaRequirements.RoleRequirement), new RoleRequirementHandler(roleHierarchyService) },
            { typeof(AcademiaRequirements.PermissionRequirement), new PermissionRequirementHandler(roleHierarchyService) },
            { typeof(AcademiaRequirements.DepartmentRequirement), new DepartmentRequirementHandler(roleHierarchyService) },
            { typeof(AcademiaRequirements.ResourceOwnershipRequirement), new ResourceOwnershipRequirementHandler(roleHierarchyService) },
            { typeof(AcademiaRequirements.MinimumRolePriorityRequirement), new MinimumRolePriorityRequirementHandler(roleHierarchyService) }
        };
    }
}