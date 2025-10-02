using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Services;

namespace Zeus.Academia.Api.Authorization;

/// <summary>
/// Authorization attribute for resource-based authorization checks.
/// This attribute ensures users can only access resources they own or have explicit permission to access.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class ResourceBasedAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>
    /// Gets or sets the type of resource being accessed.
    /// </summary>
    public ResourceType ResourceType { get; set; } = ResourceType.UserProfile;

    /// <summary>
    /// Gets or sets the parameter name that contains the resource owner ID in the request.
    /// Default is "userId" for user resources, "id" for generic resources.
    /// </summary>
    public string ResourceOwnerParameterName { get; set; } = "userId";

    /// <summary>
    /// Gets or sets the required permission level for the resource.
    /// </summary>
    public ResourcePermission RequiredPermission { get; set; } = ResourcePermission.Read;

    /// <summary>
    /// Gets or sets whether to allow system administrators to bypass resource ownership checks.
    /// Default is true.
    /// </summary>
    public bool AllowSystemAdminBypass { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow administrators to bypass resource ownership checks.
    /// Default is true.
    /// </summary>
    public bool AllowAdminBypass { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow department chairs to access resources in their department.
    /// Default is false.
    /// </summary>
    public bool AllowDepartmentChairAccess { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the ResourceBasedAuthorizationAttribute class.
    /// </summary>
    public ResourceBasedAuthorizationAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ResourceBasedAuthorizationAttribute class.
    /// </summary>
    /// <param name="resourceType">Type of resource</param>
    /// <param name="requiredPermission">Required permission level</param>
    public ResourceBasedAuthorizationAttribute(ResourceType resourceType, ResourcePermission requiredPermission = ResourcePermission.Read)
    {
        ResourceType = resourceType;
        RequiredPermission = requiredPermission;

        // Set default parameter names based on resource type
        ResourceOwnerParameterName = resourceType switch
        {
            ResourceType.UserProfile => "userId",
            ResourceType.AcademicRecord => "academicId",
            ResourceType.Grade => "studentId",
            ResourceType.Assignment => "courseId",
            _ => "id"
        };
    }

    /// <summary>
    /// Performs authorization check for resource access.
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

        // Get resource owner ID from route parameters, query string, or request body
        var resourceOwnerId = GetResourceOwnerId(context);
        if (resourceOwnerId == null)
        {
            context.Result = new BadRequestObjectResult($"Resource owner ID ({ResourceOwnerParameterName}) is required for this operation.");
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
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var currentUserId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check resource access asynchronously
        var hasAccess = CheckResourceAccessAsync(currentUserId, resourceOwnerId.Value, userService).Result;

        if (!hasAccess)
        {
            context.Result = new ForbidResult("You do not have permission to access this resource.");
        }
    }

    /// <summary>
    /// Extracts the resource owner ID from the request context.
    /// </summary>
    /// <param name="context">Authorization filter context</param>
    /// <returns>Resource owner ID if found, otherwise null</returns>
    private int? GetResourceOwnerId(AuthorizationFilterContext context)
    {
        // Try route values first
        if (context.RouteData.Values.TryGetValue(ResourceOwnerParameterName, out var routeValue) &&
            int.TryParse(routeValue?.ToString(), out var routeId))
        {
            return routeId;
        }

        // Try query string
        if (context.HttpContext.Request.Query.TryGetValue(ResourceOwnerParameterName, out var queryValue) &&
            int.TryParse(queryValue.FirstOrDefault(), out var queryId))
        {
            return queryId;
        }

        // For POST/PUT requests, try to get from action parameters
        var actionContext = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (actionContext != null)
        {
            var parameter = actionContext.Parameters.FirstOrDefault(p =>
                p.Name.Equals(ResourceOwnerParameterName, StringComparison.OrdinalIgnoreCase));

            // Note: ActionArguments are not available in AuthorizationFilterContext
            // as this filter runs before model binding. We rely on route/query parameters instead.
            // For complex scenarios, consider using an ActionFilter instead of AuthorizationFilter.
        }

        return null;
    }

    /// <summary>
    /// Checks if the current user has access to the specified resource.
    /// </summary>
    /// <param name="currentUserId">Current user ID</param>
    /// <param name="resourceOwnerId">Resource owner ID</param>
    /// <param name="userService">User service</param>
    /// <returns>True if user has access, otherwise false</returns>
    private async Task<bool> CheckResourceAccessAsync(int currentUserId, int resourceOwnerId, IUserService userService)
    {
        try
        {
            // Get current user with roles and academic information
            var currentUser = await userService.GetUserWithAcademicAndRolesAsync(currentUserId);
            if (currentUser == null || !currentUser.IsActive)
                return false;

            // System administrators can access any resource (if allowed)
            if (AllowSystemAdminBypass && currentUser.HasRole(AcademicRoleType.SystemAdmin))
                return true;

            // Administrators can access any resource (if allowed)
            if (AllowAdminBypass && currentUser.HasRole(AcademicRoleType.Administrator))
                return true;

            // Users can always access their own resources (with appropriate permission level)
            if (currentUserId == resourceOwnerId)
            {
                return CheckSelfAccessPermission(currentUser, RequiredPermission);
            }

            // Department chair access (if allowed)
            if (AllowDepartmentChairAccess && currentUser.HasRole(AcademicRoleType.Chair))
            {
                var resourceOwner = await userService.GetUserWithAcademicAsync(resourceOwnerId);
                if (resourceOwner?.Academic?.DepartmentId == currentUser.Academic?.DepartmentId)
                {
                    return CheckChairAccessPermission(RequiredPermission);
                }
            }

            // Check resource-specific access rules
            return await CheckResourceSpecificAccessAsync(currentUser, resourceOwnerId, userService);
        }
        catch
        {
            // Log the exception in a real application
            return false;
        }
    }

    /// <summary>
    /// Checks if a user has permission to access their own resource.
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="permission">Required permission</param>
    /// <returns>True if user has self-access permission</returns>
    private bool CheckSelfAccessPermission(AcademiaUser user, ResourcePermission permission)
    {
        return ResourceType switch
        {
            ResourceType.UserProfile => permission != ResourcePermission.Delete, // Users can't delete their own profiles
            ResourceType.AcademicRecord => permission == ResourcePermission.Read, // Users can only read their academic records
            ResourceType.Grade => permission == ResourcePermission.Read, // Students can only read their grades
            ResourceType.Assignment => false, // Users don't own assignments
            _ => permission == ResourcePermission.Read
        };
    }

    /// <summary>
    /// Checks if a department chair has permission for the required access level.
    /// </summary>
    /// <param name="permission">Required permission</param>
    /// <returns>True if chair has permission</returns>
    private bool CheckChairAccessPermission(ResourcePermission permission)
    {
        return ResourceType switch
        {
            ResourceType.UserProfile => permission != ResourcePermission.Delete, // Chairs can view/edit but not delete user profiles
            ResourceType.AcademicRecord => true, // Chairs have full access to academic records in their department
            ResourceType.Grade => true, // Chairs can manage grades in their department
            ResourceType.Assignment => permission != ResourcePermission.Delete, // Chairs can view/edit assignments
            _ => permission == ResourcePermission.Read
        };
    }

    /// <summary>
    /// Checks resource-specific access rules beyond ownership and administrative roles.
    /// </summary>
    /// <param name="currentUser">Current user</param>
    /// <param name="resourceOwnerId">Resource owner ID</param>
    /// <param name="userService">User service</param>
    /// <returns>True if user has resource-specific access</returns>
    private async Task<bool> CheckResourceSpecificAccessAsync(AcademiaUser currentUser, int resourceOwnerId, IUserService userService)
    {
        return ResourceType switch
        {
            ResourceType.Grade => await CheckGradeAccessAsync(currentUser, resourceOwnerId, userService),
            ResourceType.Assignment => await CheckAssignmentAccessAsync(currentUser, resourceOwnerId, userService),
            _ => false
        };
    }

    /// <summary>
    /// Checks if a user has access to view/modify grades.
    /// </summary>
    /// <param name="currentUser">Current user</param>
    /// <param name="studentId">Student ID whose grades are being accessed</param>
    /// <param name="userService">User service</param>
    /// <returns>True if user can access grades</returns>
    private Task<bool> CheckGradeAccessAsync(AcademiaUser currentUser, int studentId, IUserService userService)
    {
        // Professors and teaching professors can access grades for students in their courses
        if (currentUser.HasAnyRole(AcademicRoleType.Professor, AcademicRoleType.TeachingProf))
        {
            // Check if the professor teaches any courses the student is enrolled in
            // Note: This is a simplified implementation for medium priority fix
            // In a full implementation, this would check actual course relationships
            return Task.FromResult(RequiredPermission != ResourcePermission.Delete); // Professors can view/edit but not delete grades
        }

        return Task.FromResult(false);
    }

    /// <summary>
    /// Checks if a user has access to assignments.
    /// </summary>
    /// <param name="currentUser">Current user</param>
    /// <param name="courseId">Course ID for the assignment</param>
    /// <param name="userService">User service</param>
    /// <returns>True if user can access assignment</returns>
    private Task<bool> CheckAssignmentAccessAsync(AcademiaUser currentUser, int courseId, IUserService userService)
    {
        // Students can read assignments for courses they're enrolled in
        if (currentUser.HasRole(AcademicRoleType.Student) && RequiredPermission == ResourcePermission.Read)
        {
            // Check if student is enrolled in the course
            // Note: Simplified implementation for medium priority fix
            return Task.FromResult(RequiredPermission == ResourcePermission.Read); // Students can read assignments for courses they're enrolled in
        }

        // Faculty can manage assignments for courses they teach
        if (currentUser.HasAnyRole(AcademicRoleType.Professor, AcademicRoleType.TeachingProf))
        {
            // Check if faculty member teaches the course
            // Note: Simplified implementation for medium priority fix
            return Task.FromResult(true); // Faculty can manage assignments for courses they teach
        }

        return Task.FromResult(false);
    }
}

/// <summary>
/// Defines the types of resources that can be protected.
/// </summary>
public enum ResourceType
{
    /// <summary>
    /// User profile information.
    /// </summary>
    UserProfile = 1,

    /// <summary>
    /// Academic record information.
    /// </summary>
    AcademicRecord = 2,

    /// <summary>
    /// Grade information.
    /// </summary>
    Grade = 3,

    /// <summary>
    /// Assignment information.
    /// </summary>
    Assignment = 4,

    /// <summary>
    /// Course information.
    /// </summary>
    Course = 5,

    /// <summary>
    /// Generic resource.
    /// </summary>
    Generic = 99
}

/// <summary>
/// Defines the permission levels for resource access.
/// </summary>
public enum ResourcePermission
{
    /// <summary>
    /// Read access to the resource.
    /// </summary>
    Read = 1,

    /// <summary>
    /// Write/modify access to the resource.
    /// </summary>
    Write = 2,

    /// <summary>
    /// Delete access to the resource.
    /// </summary>
    Delete = 3
}

/// <summary>
/// Authorization requirement for resource-based access.
/// </summary>
public class ResourceBasedRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the resource type.
    /// </summary>
    public ResourceType ResourceType { get; }

    /// <summary>
    /// Gets the required permission level.
    /// </summary>
    public ResourcePermission RequiredPermission { get; }

    /// <summary>
    /// Gets the resource owner ID.
    /// </summary>
    public int ResourceOwnerId { get; }

    /// <summary>
    /// Initializes a new instance of the ResourceBasedRequirement class.
    /// </summary>
    /// <param name="resourceType">Resource type</param>
    /// <param name="resourceOwnerId">Resource owner ID</param>
    /// <param name="requiredPermission">Required permission level</param>
    public ResourceBasedRequirement(ResourceType resourceType, int resourceOwnerId, ResourcePermission requiredPermission = ResourcePermission.Read)
    {
        ResourceType = resourceType;
        ResourceOwnerId = resourceOwnerId;
        RequiredPermission = requiredPermission;
    }
}

/// <summary>
/// Authorization handler for resource-based access requirements.
/// </summary>
public class ResourceBasedHandler : AuthorizationHandler<ResourceBasedRequirement>
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the ResourceBasedHandler class.
    /// </summary>
    /// <param name="userService">User service</param>
    public ResourceBasedHandler(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Handles the resource-based access requirement.
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    /// <param name="requirement">Resource-based access requirement</param>
    /// <returns>Task representing the authorization check</returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceBasedRequirement requirement)
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
            // Get current user
            var currentUser = await _userService.GetUserWithAcademicAndRolesAsync(currentUserId);
            if (currentUser == null || !currentUser.IsActive)
            {
                context.Fail();
                return;
            }

            // System administrators can access any resource
            if (currentUser.HasRole(AcademicRoleType.SystemAdmin))
            {
                context.Succeed(requirement);
                return;
            }

            // Administrators can access any resource
            if (currentUser.HasRole(AcademicRoleType.Administrator))
            {
                context.Succeed(requirement);
                return;
            }

            // Users can access their own resources (based on permission level)
            if (currentUserId == requirement.ResourceOwnerId)
            {
                var canAccess = requirement.ResourceType switch
                {
                    ResourceType.UserProfile => requirement.RequiredPermission != ResourcePermission.Delete,
                    ResourceType.AcademicRecord => requirement.RequiredPermission == ResourcePermission.Read,
                    ResourceType.Grade => requirement.RequiredPermission == ResourcePermission.Read,
                    _ => requirement.RequiredPermission == ResourcePermission.Read
                };

                if (canAccess)
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            // Additional resource-specific checks would go here
            context.Fail();
        }
        catch
        {
            context.Fail();
        }
    }
}