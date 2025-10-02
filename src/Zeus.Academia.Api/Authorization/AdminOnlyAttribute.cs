using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Services;

namespace Zeus.Academia.Api.Authorization;

/// <summary>
/// Authorization attribute that restricts access to administrative functions only.
/// This attribute ensures only users with administrative roles can access the decorated controllers or actions.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>
    /// Gets or sets the minimum administrative level required.
    /// Default is Administrator.
    /// </summary>
    public AdminLevel MinimumLevel { get; set; } = AdminLevel.Administrator;

    /// <summary>
    /// Gets or sets whether to allow department chairs to access administrative functions.
    /// Default is false.
    /// </summary>
    public bool AllowChairs { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to check if the user account is active.
    /// Default is true.
    /// </summary>
    public bool RequireActiveAccount { get; set; } = true;

    /// <summary>
    /// Performs authorization check for administrative access.
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
        if (userService == null)
        {
            context.Result = new StatusCodeResult(500);
            return;
        }

        // Get current user
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check administrative access asynchronously
        var hasAccess = CheckAdministrativeAccessAsync(userId, userService).Result;

        if (!hasAccess)
        {
            context.Result = new ForbidResult("Administrative access required for this operation.");
        }
    }

    /// <summary>
    /// Checks if the user has the required administrative access.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="userService">User service</param>
    /// <returns>True if user has administrative access, otherwise false</returns>
    private async Task<bool> CheckAdministrativeAccessAsync(int userId, IUserService userService)
    {
        try
        {
            // Get user with roles
            var user = await userService.GetUserWithRolesAsync(userId);
            if (user == null)
                return false;

            // Check if account is active (if required)
            if (RequireActiveAccount && !user.IsActive)
                return false;

            // Check based on minimum administrative level
            return MinimumLevel switch
            {
                AdminLevel.Chair => user.HasAnyRole(AcademicRoleType.Chair, AcademicRoleType.Administrator, AcademicRoleType.SystemAdmin),
                AdminLevel.Administrator => user.HasAnyRole(AcademicRoleType.Administrator, AcademicRoleType.SystemAdmin),
                AdminLevel.SystemAdmin => user.HasRole(AcademicRoleType.SystemAdmin),
                _ => false
            } || (AllowChairs && user.HasRole(AcademicRoleType.Chair));
        }
        catch
        {
            // Log the exception in a real application
            return false;
        }
    }
}

/// <summary>
/// Defines the administrative access levels.
/// </summary>
public enum AdminLevel
{
    /// <summary>
    /// Department chair level access.
    /// </summary>
    Chair = 1,

    /// <summary>
    /// General administrator level access.
    /// </summary>
    Administrator = 2,

    /// <summary>
    /// System administrator level access (highest).
    /// </summary>
    SystemAdmin = 3
}

/// <summary>
/// Authorization requirement for administrative access.
/// </summary>
public class AdminOnlyRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the minimum administrative level required.
    /// </summary>
    public AdminLevel MinimumLevel { get; }

    /// <summary>
    /// Gets whether to allow department chairs.
    /// </summary>
    public bool AllowChairs { get; }

    /// <summary>
    /// Gets whether to require active account.
    /// </summary>
    public bool RequireActiveAccount { get; }

    /// <summary>
    /// Initializes a new instance of the AdminOnlyRequirement class.
    /// </summary>
    /// <param name="minimumLevel">Minimum administrative level</param>
    /// <param name="allowChairs">Whether to allow department chairs</param>
    /// <param name="requireActiveAccount">Whether to require active account</param>
    public AdminOnlyRequirement(AdminLevel minimumLevel = AdminLevel.Administrator, bool allowChairs = false, bool requireActiveAccount = true)
    {
        MinimumLevel = minimumLevel;
        AllowChairs = allowChairs;
        RequireActiveAccount = requireActiveAccount;
    }
}

/// <summary>
/// Authorization handler for administrative access requirements.
/// </summary>
public class AdminOnlyHandler : AuthorizationHandler<AdminOnlyRequirement>
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the AdminOnlyHandler class.
    /// </summary>
    /// <param name="userService">User service</param>
    public AdminOnlyHandler(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Handles the administrative access requirement.
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    /// <param name="requirement">Administrative access requirement</param>
    /// <returns>Task representing the authorization check</returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOnlyRequirement requirement)
    {
        // Get user ID from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            context.Fail();
            return;
        }

        try
        {
            // Get user with roles
            var user = await _userService.GetUserWithRolesAsync(userId);
            if (user == null)
            {
                context.Fail();
                return;
            }

            // Check if account is active (if required)
            if (requirement.RequireActiveAccount && !user.IsActive)
            {
                context.Fail();
                return;
            }

            // Check based on minimum administrative level
            var hasAccess = requirement.MinimumLevel switch
            {
                AdminLevel.Chair => user.HasAnyRole(AcademicRoleType.Chair, AcademicRoleType.Administrator, AcademicRoleType.SystemAdmin),
                AdminLevel.Administrator => user.HasAnyRole(AcademicRoleType.Administrator, AcademicRoleType.SystemAdmin),
                AdminLevel.SystemAdmin => user.HasRole(AcademicRoleType.SystemAdmin),
                _ => false
            } || (requirement.AllowChairs && user.HasRole(AcademicRoleType.Chair));

            if (hasAccess)
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
/// Convenience attributes for common administrative access levels.
/// </summary>

/// <summary>
/// Restricts access to system administrators only.
/// </summary>
public class SystemAdminOnlyAttribute : AdminOnlyAttribute
{
    /// <summary>
    /// Initializes a new instance of the SystemAdminOnlyAttribute class.
    /// </summary>
    public SystemAdminOnlyAttribute()
    {
        MinimumLevel = AdminLevel.SystemAdmin;
        AllowChairs = false;
    }
}

/// <summary>
/// Restricts access to administrators and system administrators.
/// </summary>
public class AdministratorOnlyAttribute : AdminOnlyAttribute
{
    /// <summary>
    /// Initializes a new instance of the AdministratorOnlyAttribute class.
    /// </summary>
    public AdministratorOnlyAttribute()
    {
        MinimumLevel = AdminLevel.Administrator;
        AllowChairs = false;
    }
}

/// <summary>
/// Restricts access to department chairs and higher administrative roles.
/// </summary>
public class ChairOrHigherAttribute : AdminOnlyAttribute
{
    /// <summary>
    /// Initializes a new instance of the ChairOrHigherAttribute class.
    /// </summary>
    public ChairOrHigherAttribute()
    {
        MinimumLevel = AdminLevel.Chair;
        AllowChairs = true;
    }
}