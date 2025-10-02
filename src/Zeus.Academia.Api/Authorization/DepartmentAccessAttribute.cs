using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Services;

namespace Zeus.Academia.Api.Authorization;

/// <summary>
/// Authorization attribute that ensures the user has access to a specific department.
/// This attribute checks if the user belongs to the department or has administrative privileges.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class DepartmentAccessAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>
    /// Gets or sets the parameter name that contains the department ID in the request.
    /// Default is "departmentId".
    /// </summary>
    public string DepartmentIdParameterName { get; set; } = "departmentId";

    /// <summary>
    /// Gets or sets whether to allow system administrators to bypass department access checks.
    /// Default is true.
    /// </summary>
    public bool AllowSystemAdminBypass { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow department chairs to access other departments.
    /// Default is false.
    /// </summary>
    public bool AllowChairCrossDepartmentAccess { get; set; } = false;

    /// <summary>
    /// Performs authorization check for department access.
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

        // Get department ID from route parameters, query string, or request body
        var departmentId = GetDepartmentId(context);
        if (departmentId == null)
        {
            context.Result = new BadRequestObjectResult("Department ID is required for this operation.");
            return;
        }

        // Get user service to check department access
        var userService = context.HttpContext.RequestServices.GetService<IUserService>();
        var roleHierarchyService = context.HttpContext.RequestServices.GetService<IRoleHierarchyService>();

        if (userService == null || roleHierarchyService == null)
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

        // Check department access asynchronously
        var hasAccess = CheckDepartmentAccessAsync(userId, departmentId.Value, userService, roleHierarchyService).Result;

        if (!hasAccess)
        {
            context.Result = new ForbidResult("You do not have access to this department.");
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
        if (context.RouteData.Values.TryGetValue(DepartmentIdParameterName, out var routeValue) &&
            int.TryParse(routeValue?.ToString(), out var routeDeptId))
        {
            return routeDeptId;
        }

        // Try query string
        if (context.HttpContext.Request.Query.TryGetValue(DepartmentIdParameterName, out var queryValue) &&
            int.TryParse(queryValue.FirstOrDefault(), out var queryDeptId))
        {
            return queryDeptId;
        }

        // For POST/PUT requests, try to get from action parameters
        var actionContext = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (actionContext != null)
        {
            var parameter = actionContext.Parameters.FirstOrDefault(p =>
                p.Name.Equals(DepartmentIdParameterName, StringComparison.OrdinalIgnoreCase));

            // Note: ActionArguments are not available in AuthorizationFilterContext
            // as this filter runs before model binding. We rely on route/query parameters instead.
            // For complex scenarios, consider using an ActionFilter instead of AuthorizationFilter.
        }

        return null;
    }

    /// <summary>
    /// Checks if the user has access to the specified department.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">Department ID</param>
    /// <param name="userService">User service</param>
    /// <param name="roleHierarchyService">Role hierarchy service</param>
    /// <returns>True if user has access, otherwise false</returns>
    private async Task<bool> CheckDepartmentAccessAsync(int userId, int departmentId, IUserService userService, IRoleHierarchyService roleHierarchyService)
    {
        try
        {
            // Get user with academic information
            var user = await userService.GetUserWithAcademicAsync(userId);
            if (user == null || !user.IsActive)
                return false;

            // System administrators can access any department (if allowed)
            if (AllowSystemAdminBypass && user.HasRole(AcademicRoleType.SystemAdmin))
                return true;

            // Check if user belongs to the department
            if (user.Academic?.DepartmentId == departmentId)
                return true;

            // Chair cross-department access (if allowed)
            if (AllowChairCrossDepartmentAccess && user.HasRole(AcademicRoleType.Chair))
                return true;

            // Administrators can access any department
            if (user.HasRole(AcademicRoleType.Administrator))
                return true;

            return false;
        }
        catch
        {
            // Log the exception in a real application
            return false;
        }
    }
}

/// <summary>
/// Authorization requirement for department access.
/// </summary>
public class DepartmentAccessRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the required department ID.
    /// </summary>
    public int DepartmentId { get; }

    /// <summary>
    /// Gets whether to allow system administrator bypass.
    /// </summary>
    public bool AllowSystemAdminBypass { get; }

    /// <summary>
    /// Initializes a new instance of the DepartmentAccessRequirement class.
    /// </summary>
    /// <param name="departmentId">Required department ID</param>
    /// <param name="allowSystemAdminBypass">Whether to allow system admin bypass</param>
    public DepartmentAccessRequirement(int departmentId, bool allowSystemAdminBypass = true)
    {
        DepartmentId = departmentId;
        AllowSystemAdminBypass = allowSystemAdminBypass;
    }
}

/// <summary>
/// Authorization handler for department access requirements.
/// </summary>
public class DepartmentAccessHandler : AuthorizationHandler<DepartmentAccessRequirement>
{
    private readonly IUserService _userService;
    private readonly IRoleHierarchyService _roleHierarchyService;

    /// <summary>
    /// Initializes a new instance of the DepartmentAccessHandler class.
    /// </summary>
    /// <param name="userService">User service</param>
    /// <param name="roleHierarchyService">Role hierarchy service</param>
    public DepartmentAccessHandler(IUserService userService, IRoleHierarchyService roleHierarchyService)
    {
        _userService = userService;
        _roleHierarchyService = roleHierarchyService;
    }

    /// <summary>
    /// Handles the department access requirement.
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    /// <param name="requirement">Department access requirement</param>
    /// <returns>Task representing the authorization check</returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DepartmentAccessRequirement requirement)
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
            // Get user with academic information
            var user = await _userService.GetUserWithAcademicAsync(userId);
            if (user == null || !user.IsActive)
            {
                context.Fail();
                return;
            }

            // System administrators can access any department (if allowed)
            if (requirement.AllowSystemAdminBypass && user.HasRole(AcademicRoleType.SystemAdmin))
            {
                context.Succeed(requirement);
                return;
            }

            // Check if user belongs to the department
            if (user.Academic?.DepartmentId == requirement.DepartmentId)
            {
                context.Succeed(requirement);
                return;
            }

            // Administrators can access any department
            if (user.HasRole(AcademicRoleType.Administrator))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
        catch
        {
            context.Fail();
        }
    }
}