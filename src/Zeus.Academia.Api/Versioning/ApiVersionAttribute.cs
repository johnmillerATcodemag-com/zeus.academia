using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zeus.Academia.Api.Versioning;

/// <summary>
/// Attribute to specify API version for controllers
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class ApiVersionAttribute : Attribute, IActionFilter
{
    /// <summary>
    /// Gets the API version
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets whether this version is deprecated
    /// </summary>
    public bool IsDeprecated { get; set; }

    /// <summary>
    /// Initializes a new instance of the ApiVersionAttribute
    /// </summary>
    /// <param name="version">The API version (e.g., "1.0", "2.0")</param>
    public ApiVersionAttribute(string version)
    {
        Version = version ?? throw new ArgumentNullException(nameof(version));
    }

    /// <summary>
    /// Called before the action method is invoked
    /// </summary>
    /// <param name="context">The action executing context</param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var requestedVersion = GetRequestedVersion(context.HttpContext);

        if (string.IsNullOrEmpty(requestedVersion))
        {
            // Default to this version if no version specified
            context.HttpContext.Items["ApiVersion"] = Version;
            return;
        }

        if (!string.Equals(requestedVersion, Version, StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new BadRequestObjectResult(new
            {
                Error = "Version Mismatch",
                Message = $"Requested version '{requestedVersion}' does not match controller version '{Version}'",
                RequestedVersion = requestedVersion,
                AvailableVersion = Version
            });
            return;
        }

        context.HttpContext.Items["ApiVersion"] = Version;

        if (IsDeprecated)
        {
            context.HttpContext.Response.Headers["X-API-Deprecated"] = "true";
            context.HttpContext.Response.Headers["X-API-Deprecation-Message"] =
                $"API version {Version} is deprecated. Please upgrade to a newer version.";
        }
    }

    /// <summary>
    /// Called after the action method is invoked
    /// </summary>
    /// <param name="context">The action executed context</param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Add version information to response headers
        context.HttpContext.Response.Headers["X-API-Version"] = Version;
    }

    /// <summary>
    /// Gets the requested API version from the request
    /// </summary>
    /// <param name="httpContext">The HTTP context</param>
    /// <returns>The requested version or null if not specified</returns>
    private static string? GetRequestedVersion(HttpContext httpContext)
    {
        // Check X-API-Version header first
        if (httpContext.Request.Headers.TryGetValue("X-API-Version", out var headerValue))
        {
            return headerValue.FirstOrDefault();
        }

        // Check query parameter as fallback
        if (httpContext.Request.Query.TryGetValue("version", out var queryValue))
        {
            return queryValue.FirstOrDefault();
        }

        // Check route data for URL-based versioning support
        if (httpContext.Request.RouteValues.TryGetValue("version", out var routeValue))
        {
            return routeValue?.ToString();
        }

        return null;
    }
}