using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Zeus.Academia.Api.Middleware;

/// <summary>
/// Middleware for logging security-related events and requests.
/// Captures authentication attempts, authorization failures, and sensitive operations.
/// </summary>
public class SecurityLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityLoggingMiddleware> _logger;

    public SecurityLoggingMiddleware(RequestDelegate next, ILogger<SecurityLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var originalBodyStream = context.Response.Body;

        try
        {
            // Log incoming request for security-sensitive endpoints
            await LogSecurityRequest(context);

            // Capture response for sensitive endpoints
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            stopwatch.Stop();

            // Log security events based on response
            await LogSecurityResponse(context, stopwatch.ElapsedMilliseconds);

            // Copy response back to original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await LogSecurityException(context, ex, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private Task LogSecurityRequest(HttpContext context)
    {
        if (!IsSecurityRelevantRequest(context))
            return Task.CompletedTask;

        var requestInfo = new
        {
            Timestamp = DateTime.UtcNow,
            Method = context.Request.Method,
            Path = context.Request.Path.Value,
            QueryString = context.Request.QueryString.Value,
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            RemoteIpAddress = GetClientIpAddress(context),
            UserId = GetUserId(context),
            UserName = GetUserName(context),
            IsAuthenticated = context.User.Identity?.IsAuthenticated ?? false,
            Roles = GetUserRoles(context)
        };

        // Log request details for security monitoring
        _logger.LogInformation("Security Request: {@RequestInfo}", requestInfo);

        // Log potential suspicious activity
        if (IsSuspiciousRequest(context))
        {
            _logger.LogWarning("Suspicious Request Detected: {@RequestInfo}", requestInfo);
        }

        return Task.CompletedTask;
    }

    private Task LogSecurityResponse(HttpContext context, long elapsedMs)
    {
        if (!IsSecurityRelevantRequest(context))
            return Task.CompletedTask;

        var responseInfo = new
        {
            Timestamp = DateTime.UtcNow,
            StatusCode = context.Response.StatusCode,
            Path = context.Request.Path.Value,
            ElapsedMs = elapsedMs,
            UserId = GetUserId(context),
            RemoteIpAddress = GetClientIpAddress(context)
        };

        // Log based on response status
        switch (context.Response.StatusCode)
        {
            case 200:
            case 201:
                if (IsAuthenticationEndpoint(context.Request.Path))
                {
                    _logger.LogInformation("Successful Authentication: {@ResponseInfo}", responseInfo);
                }
                else if (IsSensitiveOperation(context))
                {
                    _logger.LogInformation("Successful Sensitive Operation: {@ResponseInfo}", responseInfo);
                }
                break;

            case 401:
                _logger.LogWarning("Authentication Failed: {@ResponseInfo}", responseInfo);
                break;

            case 403:
                _logger.LogWarning("Authorization Failed: {@ResponseInfo}", responseInfo);
                break;

            case 429:
                _logger.LogWarning("Rate Limit Exceeded: {@ResponseInfo}", responseInfo);
                break;

            default:
                if (context.Response.StatusCode >= 400)
                {
                    _logger.LogWarning("Security Error Response: {@ResponseInfo}", responseInfo);
                }
                break;
        }

        return Task.CompletedTask;
    }

    private Task LogSecurityException(HttpContext context, Exception exception, long elapsedMs)
    {
        var exceptionInfo = new
        {
            Timestamp = DateTime.UtcNow,
            Exception = exception.GetType().Name,
            Message = exception.Message,
            Path = context.Request.Path.Value,
            ElapsedMs = elapsedMs,
            UserId = GetUserId(context),
            RemoteIpAddress = GetClientIpAddress(context)
        };

        _logger.LogError(exception, "Security Exception: {@ExceptionInfo}", exceptionInfo);
        return Task.CompletedTask;
    }

    private static bool IsSecurityRelevantRequest(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        return path != null && (
            path.Contains("/auth/") ||
            path.Contains("/user/") ||
            path.Contains("/admin/") ||
            path.Contains("/token/") ||
            path.Contains("/password/") ||
            path.Contains("/role/") ||
            context.Request.Method != "GET"
        );
    }

    private static bool IsAuthenticationEndpoint(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant();
        return pathValue != null && (
            pathValue.Contains("/auth/login") ||
            pathValue.Contains("/auth/token") ||
            pathValue.Contains("/auth/refresh")
        );
    }

    private static bool IsSensitiveOperation(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        return path != null && (
            path.Contains("/admin/") ||
            path.Contains("/user/") && (context.Request.Method == "POST" ||
                                       context.Request.Method == "PUT" ||
                                       context.Request.Method == "DELETE") ||
            path.Contains("/role/") ||
            path.Contains("/password/")
        );
    }

    private static bool IsSuspiciousRequest(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        var path = context.Request.Path.Value?.ToLowerInvariant();

        // Check for suspicious patterns
        return string.IsNullOrEmpty(userAgent) ||
               userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
               path?.Contains("..") == true ||
               path?.Contains("script") == true ||
               context.Request.Headers.Count > 50; // Unusually high header count
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        // Check X-Forwarded-For header first (for load balancers/proxies)
        var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            return xForwardedFor.Split(',').FirstOrDefault()?.Trim();
        }

        // Check X-Real-IP header
        var xRealIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp))
        {
            return xRealIp;
        }

        // Fall back to connection remote IP
        return context.Connection.RemoteIpAddress?.ToString();
    }

    private static string? GetUserId(HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    private static string? GetUserName(HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.Name)?.Value;
    }

    private static string[] GetUserRoles(HttpContext context)
    {
        return context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
    }
}

/// <summary>
/// Extension methods for registering SecurityLoggingMiddleware.
/// </summary>
public static class SecurityLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds the SecurityLoggingMiddleware to the application pipeline.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseSecurityLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityLoggingMiddleware>();
    }
}