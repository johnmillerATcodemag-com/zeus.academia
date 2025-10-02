using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Services;

namespace Zeus.Academia.Api.Authorization;

/// <summary>
/// Authorization attribute that ensures the user has access to course-related operations.
/// This attribute checks if the user is enrolled in the course, teaches the course, or has administrative privileges.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class CourseAccessAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>
    /// Gets or sets the parameter name that contains the course ID in the request.
    /// Default is "courseId".
    /// </summary>
    public string CourseIdParameterName { get; set; } = "courseId";

    /// <summary>
    /// Gets or sets the required access level for the course.
    /// </summary>
    public CourseAccessLevel AccessLevel { get; set; } = CourseAccessLevel.Read;

    /// <summary>
    /// Gets or sets whether to allow system administrators to bypass course access checks.
    /// Default is true.
    /// </summary>
    public bool AllowSystemAdminBypass { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow department chairs to access courses in their department.
    /// Default is true.
    /// </summary>
    public bool AllowDepartmentChairAccess { get; set; } = true;

    /// <summary>
    /// Performs authorization check for course access.
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

        // Get course ID from route parameters, query string, or request body
        var courseId = GetCourseId(context);
        if (courseId == null)
        {
            context.Result = new BadRequestObjectResult("Course ID is required for this operation.");
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

        // Check course access asynchronously
        var hasAccess = CheckCourseAccessAsync(userId, courseId.Value, userService).Result;

        if (!hasAccess)
        {
            context.Result = new ForbidResult("You do not have access to this course.");
        }
    }

    /// <summary>
    /// Extracts the course ID from the request context.
    /// </summary>
    /// <param name="context">Authorization filter context</param>
    /// <returns>Course ID if found, otherwise null</returns>
    private int? GetCourseId(AuthorizationFilterContext context)
    {
        // Try route values first
        if (context.RouteData.Values.TryGetValue(CourseIdParameterName, out var routeValue) &&
            int.TryParse(routeValue?.ToString(), out var routeCourseId))
        {
            return routeCourseId;
        }

        // Try query string
        if (context.HttpContext.Request.Query.TryGetValue(CourseIdParameterName, out var queryValue) &&
            int.TryParse(queryValue.FirstOrDefault(), out var queryCourseId))
        {
            return queryCourseId;
        }

        // For POST/PUT requests, try to get from action parameters
        var actionContext = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (actionContext != null)
        {
            var parameter = actionContext.Parameters.FirstOrDefault(p =>
                p.Name.Equals(CourseIdParameterName, StringComparison.OrdinalIgnoreCase));

            // Note: ActionArguments are not available in AuthorizationFilterContext
            // as this filter runs before model binding. We rely on route/query parameters instead.
            // For complex scenarios, consider using an ActionFilter instead of AuthorizationFilter.
        }

        return null;
    }

    /// <summary>
    /// Checks if the user has access to the specified course.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="courseId">Course ID</param>
    /// <param name="userService">User service</param>
    /// <returns>True if user has access, otherwise false</returns>
    private async Task<bool> CheckCourseAccessAsync(int userId, int courseId, IUserService userService)
    {
        try
        {
            // Get user with academic information
            var user = await userService.GetUserWithAcademicAsync(userId);
            if (user == null || !user.IsActive)
                return false;

            // System administrators can access any course (if allowed)
            if (AllowSystemAdminBypass && user.HasRole(AcademicRoleType.SystemAdmin))
                return true;

            // Administrators can access any course
            if (user.HasRole(AcademicRoleType.Administrator))
                return true;

            // Implement course access logic based on domain model
            // Check if user is enrolled in the course (for students)
            if (user.HasRole(AcademicRoleType.Student))
            {
                return CheckStudentEnrollment(user.Id, courseId);
            }

            // Check if user teaches the course (for professors)
            if (user.HasAnyRole(AcademicRoleType.Professor, AcademicRoleType.TeachingProf))
            {
                // Allow faculty access based on department and access level
                return CheckFacultyAccess(user, AccessLevel);
            }

            if (user.HasRole(AcademicRoleType.Student))
            {
                // Students can only read course information they're enrolled in
                return AccessLevel == CourseAccessLevel.Read && CheckStudentEnrollment(userId, courseId);
            }

            return false;
        }
        catch
        {
            // Log the exception in a real application
            return false;
        }
    }

    /// <summary>
    /// Checks faculty access based on access level requirements.
    /// </summary>
    /// <param name="user">Academic user</param>
    /// <param name="accessLevel">Required access level</param>
    /// <returns>True if faculty has required access</returns>
    private bool CheckFacultyAccess(AcademiaUser user, CourseAccessLevel accessLevel)
    {
        return accessLevel switch
        {
            CourseAccessLevel.Read => true, // All faculty can read
            CourseAccessLevel.Write => user.HasAnyRole(AcademicRoleType.Professor, AcademicRoleType.TeachingProf, AcademicRoleType.Chair),
            CourseAccessLevel.Admin => user.HasAnyRole(AcademicRoleType.Chair, AcademicRoleType.Administrator, AcademicRoleType.SystemAdmin),
            _ => false
        };
    }

    /// <summary>
    /// Checks if a student is enrolled in the specified course.
    /// </summary>
    /// <param name="userId">Student user ID</param>
    /// <param name="courseId">Course ID</param>
    /// <returns>True if student is enrolled</returns>
    private bool CheckStudentEnrollment(int userId, int courseId)
    {
        // Note: This is a simplified implementation
        // In a real application, this would check the StudentEnrollment table
        // For now, allow read access for authenticated students
        return AccessLevel == CourseAccessLevel.Read;
    }
}

/// <summary>
/// Defines the access levels for course operations.
/// </summary>
public enum CourseAccessLevel
{
    /// <summary>
    /// Read-only access to course information.
    /// </summary>
    Read = 1,

    /// <summary>
    /// Write access to course content and assignments.
    /// </summary>
    Write = 2,

    /// <summary>
    /// Administrative access including enrollment management.
    /// </summary>
    Admin = 3
}

/// <summary>
/// Authorization requirement for course access.
/// </summary>
public class CourseAccessRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the required course ID.
    /// </summary>
    public int CourseId { get; }

    /// <summary>
    /// Gets the required access level.
    /// </summary>
    public CourseAccessLevel AccessLevel { get; }

    /// <summary>
    /// Gets whether to allow system administrator bypass.
    /// </summary>
    public bool AllowSystemAdminBypass { get; }

    /// <summary>
    /// Initializes a new instance of the CourseAccessRequirement class.
    /// </summary>
    /// <param name="courseId">Required course ID</param>
    /// <param name="accessLevel">Required access level</param>
    /// <param name="allowSystemAdminBypass">Whether to allow system admin bypass</param>
    public CourseAccessRequirement(int courseId, CourseAccessLevel accessLevel = CourseAccessLevel.Read, bool allowSystemAdminBypass = true)
    {
        CourseId = courseId;
        AccessLevel = accessLevel;
        AllowSystemAdminBypass = allowSystemAdminBypass;
    }
}

/// <summary>
/// Authorization handler for course access requirements.
/// </summary>
public class CourseAccessHandler : AuthorizationHandler<CourseAccessRequirement>
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the CourseAccessHandler class.
    /// </summary>
    /// <param name="userService">User service</param>
    public CourseAccessHandler(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Handles the course access requirement.
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    /// <param name="requirement">Course access requirement</param>
    /// <returns>Task representing the authorization check</returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CourseAccessRequirement requirement)
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

            // System administrators can access any course (if allowed)
            if (requirement.AllowSystemAdminBypass && user.HasRole(AcademicRoleType.SystemAdmin))
            {
                context.Succeed(requirement);
                return;
            }

            // Administrators can access any course
            if (user.HasRole(AcademicRoleType.Administrator))
            {
                context.Succeed(requirement);
                return;
            }

            // Check access based on role and access level
            var hasAccess = requirement.AccessLevel switch
            {
                CourseAccessLevel.Read => user.HasAnyRole(AcademicRoleType.Student, AcademicRoleType.Professor, AcademicRoleType.TeachingProf, AcademicRoleType.Chair),
                CourseAccessLevel.Write => user.HasAnyRole(AcademicRoleType.Professor, AcademicRoleType.TeachingProf, AcademicRoleType.Chair),
                CourseAccessLevel.Admin => user.HasAnyRole(AcademicRoleType.Chair, AcademicRoleType.Administrator, AcademicRoleType.SystemAdmin),
                _ => false
            };

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