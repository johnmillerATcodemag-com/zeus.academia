using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Services;

namespace Zeus.Academia.Api.Authorization;

/// <summary>
/// Authorization attribute that enforces role hierarchy relationships.
/// This attribute ensures users have the appropriate role level and validates role inheritance.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RoleHierarchyAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>
    /// Gets or sets the minimum required role for access.
    /// </summary>
    public AcademicRoleType MinimumRole { get; set; }

    /// <summary>
    /// Gets or sets the exact role required (no hierarchy checking).
    /// When set, only users with this exact role can access the resource.
    /// </summary>
    public AcademicRoleType? ExactRole { get; set; }

    /// <summary>
    /// Gets or sets whether to allow role inheritance.
    /// When true, higher-level roles can access resources for lower-level roles.
    /// Default is true.
    /// </summary>
    public bool AllowRoleInheritance { get; set; } = true;

    /// <summary>
    /// Gets or sets additional roles that are allowed access (beyond hierarchy).
    /// </summary>
    public AcademicRoleType[] AdditionalAllowedRoles { get; set; } = Array.Empty<AcademicRoleType>();

    /// <summary>
    /// Gets or sets roles that are explicitly denied access.
    /// These roles are denied even if they would normally be allowed by hierarchy.
    /// </summary>
    public AcademicRoleType[] DeniedRoles { get; set; } = Array.Empty<AcademicRoleType>();

    /// <summary>
    /// Gets or sets whether to require active account status.
    /// Default is true.
    /// </summary>
    public bool RequireActiveAccount { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to require department-specific access.
    /// When true, checks if the user's department matches a department parameter in the request.
    /// </summary>
    public bool RequireDepartmentMatch { get; set; } = false;

    /// <summary>
    /// Gets or sets the parameter name for department matching.
    /// Used when RequireDepartmentMatch is true.
    /// </summary>
    public string DepartmentParameterName { get; set; } = "departmentId";

    /// <summary>
    /// Initializes a new instance of the RoleHierarchyAttribute class.
    /// </summary>
    /// <param name="minimumRole">Minimum required role</param>
    public RoleHierarchyAttribute(AcademicRoleType minimumRole)
    {
        MinimumRole = minimumRole;
    }

    /// <summary>
    /// Initializes a new instance of the RoleHierarchyAttribute class for exact role matching.
    /// </summary>
    /// <param name="exactRole">Exact role required</param>
    /// <param name="allowInheritance">Whether to allow role inheritance</param>
    public RoleHierarchyAttribute(AcademicRoleType exactRole, bool allowInheritance)
    {
        ExactRole = exactRole;
        MinimumRole = exactRole;
        AllowRoleInheritance = allowInheritance;
    }

    /// <summary>
    /// Performs authorization check based on role hierarchy.
    /// </summary>
    /// <param name="context">Authorization filter context</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get required services
        var userService = context.HttpContext.RequestServices.GetService<IUserService>();
        var roleHierarchyService = context.HttpContext.RequestServices.GetService<IRoleHierarchyService>();

        if (userService == null || roleHierarchyService == null)
        {
            context.Result = new StatusCodeResult(500);
            return;
        }

        // Get current user
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var currentUserId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check role hierarchy access asynchronously
        var hasAccess = CheckRoleHierarchyAccessAsync(currentUserId, context, userService, roleHierarchyService).Result;

        if (!hasAccess)
        {
            context.Result = new ForbidResult("You do not have the required role permissions to access this resource.");
        }
    }

    /// <summary>
    /// Checks if the current user has the required role hierarchy access.
    /// </summary>
    /// <param name="currentUserId">Current user ID</param>
    /// <param name="context">Authorization filter context</param>
    /// <param name="userService">User service</param>
    /// <param name="roleHierarchyService">Role hierarchy service</param>
    /// <returns>True if user has access, otherwise false</returns>
    private async Task<bool> CheckRoleHierarchyAccessAsync(int currentUserId, AuthorizationFilterContext context,
        IUserService userService, IRoleHierarchyService roleHierarchyService)
    {
        try
        {
            // Get current user with roles and academic information
            var currentUser = await userService.GetUserWithAcademicAndRolesAsync(currentUserId);
            if (currentUser == null)
                return false;

            // Check active account requirement
            if (RequireActiveAccount && !currentUser.IsActive)
                return false;

            // Check explicitly denied roles first
            if (DeniedRoles.Any(role => currentUser.HasRole(role)))
                return false;

            // Check exact role requirement (if specified)
            if (ExactRole.HasValue)
            {
                if (!AllowRoleInheritance)
                {
                    // Must have exact role, no inheritance
                    return currentUser.HasRole(ExactRole.Value);
                }
                else
                {
                    // Check if user has exact role or higher in hierarchy
                    return roleHierarchyService.HasRoleOrHigher(currentUser, ExactRole.Value);
                }
            }

            // Check minimum role requirement with hierarchy
            bool hasMinimumRole = false;
            if (AllowRoleInheritance)
            {
                hasMinimumRole = roleHierarchyService.HasRoleOrHigher(currentUser, MinimumRole);
            }
            else
            {
                hasMinimumRole = currentUser.HasRole(MinimumRole);
            }

            // Check additional allowed roles
            bool hasAdditionalRole = AdditionalAllowedRoles.Length == 0 ||
                AdditionalAllowedRoles.Any(role => currentUser.HasRole(role));

            // User must have minimum role OR be in additional allowed roles
            if (!hasMinimumRole && !hasAdditionalRole)
                return false;

            // Check department match requirement
            if (RequireDepartmentMatch)
            {
                var departmentId = GetDepartmentId(context);
                if (departmentId.HasValue && currentUser.Academic?.DepartmentId != departmentId.Value)
                {
                    // Allow system admins and administrators to bypass department restrictions
                    if (!currentUser.HasAnyRole(AcademicRoleType.SystemAdmin, AcademicRoleType.Administrator))
                        return false;
                }
            }

            return true;
        }
        catch
        {
            // Log the exception in a real application
            return false;
        }
    }

    /// <summary>
    /// Extracts the department ID from the request context.
    /// </summary>
    /// <param name="context">Authorization filter context</param>
    /// <returns>Department ID if found, otherwise null</returns>
    private int? GetDepartmentId(AuthorizationFilterContext context)
    {
        // Try route values first
        if (context.RouteData.Values.TryGetValue(DepartmentParameterName, out var routeValue) &&
            int.TryParse(routeValue?.ToString(), out var routeId))
        {
            return routeId;
        }

        // Try query string
        if (context.HttpContext.Request.Query.TryGetValue(DepartmentParameterName, out var queryValue) &&
            int.TryParse(queryValue.FirstOrDefault(), out var queryId))
        {
            return queryId;
        }

        return null;
    }
}

/// <summary>
/// Authorization requirement for role hierarchy checks.
/// </summary>
public class RoleHierarchyRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the minimum required role.
    /// </summary>
    public AcademicRoleType MinimumRole { get; }

    /// <summary>
    /// Gets the exact role required (if any).
    /// </summary>
    public AcademicRoleType? ExactRole { get; }

    /// <summary>
    /// Gets whether role inheritance is allowed.
    /// </summary>
    public bool AllowRoleInheritance { get; }

    /// <summary>
    /// Gets additional allowed roles.
    /// </summary>
    public AcademicRoleType[] AdditionalAllowedRoles { get; }

    /// <summary>
    /// Gets denied roles.
    /// </summary>
    public AcademicRoleType[] DeniedRoles { get; }

    /// <summary>
    /// Initializes a new instance of the RoleHierarchyRequirement class.
    /// </summary>
    /// <param name="minimumRole">Minimum required role</param>
    /// <param name="exactRole">Exact role required</param>
    /// <param name="allowRoleInheritance">Allow role inheritance</param>
    /// <param name="additionalAllowedRoles">Additional allowed roles</param>
    /// <param name="deniedRoles">Denied roles</param>
    public RoleHierarchyRequirement(AcademicRoleType minimumRole, AcademicRoleType? exactRole = null,
        bool allowRoleInheritance = true, AcademicRoleType[]? additionalAllowedRoles = null,
        AcademicRoleType[]? deniedRoles = null)
    {
        MinimumRole = minimumRole;
        ExactRole = exactRole;
        AllowRoleInheritance = allowRoleInheritance;
        AdditionalAllowedRoles = additionalAllowedRoles ?? Array.Empty<AcademicRoleType>();
        DeniedRoles = deniedRoles ?? Array.Empty<AcademicRoleType>();
    }
}

/// <summary>
/// Authorization handler for role hierarchy requirements.
/// </summary>
public class RoleHierarchyHandler : AuthorizationHandler<RoleHierarchyRequirement>
{
    private readonly IUserService _userService;
    private readonly IRoleHierarchyService _roleHierarchyService;

    /// <summary>
    /// Initializes a new instance of the RoleHierarchyHandler class.
    /// </summary>
    /// <param name="userService">User service</param>
    /// <param name="roleHierarchyService">Role hierarchy service</param>
    public RoleHierarchyHandler(IUserService userService, IRoleHierarchyService roleHierarchyService)
    {
        _userService = userService;
        _roleHierarchyService = roleHierarchyService;
    }

    /// <summary>
    /// Handles the role hierarchy requirement.
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    /// <param name="requirement">Role hierarchy requirement</param>
    /// <returns>Task representing the authorization check</returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleHierarchyRequirement requirement)
    {
        // Get user ID from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var currentUserId))
        {
            context.Fail();
            return;
        }

        try
        {
            // Get current user with roles
            var currentUser = await _userService.GetUserWithAcademicAndRolesAsync(currentUserId);
            if (currentUser == null || !currentUser.IsActive)
            {
                context.Fail();
                return;
            }

            // Check explicitly denied roles first
            if (requirement.DeniedRoles.Any(role => currentUser.HasRole(role)))
            {
                context.Fail();
                return;
            }

            // Check exact role requirement (if specified)
            if (requirement.ExactRole.HasValue)
            {
                if (!requirement.AllowRoleInheritance)
                {
                    if (currentUser.HasRole(requirement.ExactRole.Value))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
                else
                {
                    if (_roleHierarchyService.HasRoleOrHigher(currentUser, requirement.ExactRole.Value))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }

            // Check minimum role requirement
            bool hasMinimumRole = false;
            if (requirement.AllowRoleInheritance)
            {
                hasMinimumRole = _roleHierarchyService.HasRoleOrHigher(currentUser, requirement.MinimumRole);
            }
            else
            {
                hasMinimumRole = currentUser.HasRole(requirement.MinimumRole);
            }

            // Check additional allowed roles
            bool hasAdditionalRole = requirement.AdditionalAllowedRoles.Length == 0 ||
                requirement.AdditionalAllowedRoles.Any(role => currentUser.HasRole(role));

            // User must have minimum role OR be in additional allowed roles
            if (hasMinimumRole || hasAdditionalRole)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
        catch
        {
            context.Fail();
        }
    }
}

/// <summary>
/// Convenience attribute for student-only access.
/// </summary>
public class StudentOnlyAttribute : RoleHierarchyAttribute
{
    /// <summary>
    /// Initializes a new instance of the StudentOnlyAttribute class.
    /// </summary>
    public StudentOnlyAttribute() : base(AcademicRoleType.Student, false)
    {
    }
}

/// <summary>
/// Convenience attribute for faculty-only access (Professor or TeachingProf).
/// </summary>
public class FacultyOnlyAttribute : RoleHierarchyAttribute
{
    /// <summary>
    /// Initializes a new instance of the FacultyOnlyAttribute class.
    /// </summary>
    public FacultyOnlyAttribute() : base(AcademicRoleType.TeachingProf)
    {
        AdditionalAllowedRoles = new[] { AcademicRoleType.Professor };
    }
}

/// <summary>
/// Convenience attribute for professor or higher access.
/// </summary>
public class ProfessorOrHigherAttribute : RoleHierarchyAttribute
{
    /// <summary>
    /// Initializes a new instance of the ProfessorOrHigherAttribute class.
    /// </summary>
    public ProfessorOrHigherAttribute() : base(AcademicRoleType.Professor)
    {
    }
}

/// <summary>
/// Convenience attribute for teaching professor or higher access.
/// </summary>
public class TeachingProfOrHigherAttribute : RoleHierarchyAttribute
{
    /// <summary>
    /// Initializes a new instance of the TeachingProfOrHigherAttribute class.
    /// </summary>
    public TeachingProfOrHigherAttribute() : base(AcademicRoleType.TeachingProf)
    {
    }
}

/// <summary>
/// Convenience attribute for academic staff access (no students).
/// </summary>
public class AcademicStaffOnlyAttribute : RoleHierarchyAttribute
{
    /// <summary>
    /// Initializes a new instance of the AcademicStaffOnlyAttribute class.
    /// </summary>
    public AcademicStaffOnlyAttribute() : base(AcademicRoleType.TeachingProf)
    {
        DeniedRoles = new[] { AcademicRoleType.Student };
    }
}