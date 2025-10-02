using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Zeus.Academia.Api.Middleware;

/// <summary>
/// Middleware to add security headers to HTTP responses.
/// Implements OWASP recommended security headers to protect against common web vulnerabilities.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers to the response
        AddSecurityHeaders(context);

        await _next(context);
    }

    private void AddSecurityHeaders(HttpContext context)
    {
        var headers = context.Response.Headers;

        try
        {
            // Prevent clickjacking attacks
            if (!headers.ContainsKey("X-Frame-Options"))
            {
                headers["X-Frame-Options"] = "DENY";
            }

            // Prevent MIME type sniffing
            if (!headers.ContainsKey("X-Content-Type-Options"))
            {
                headers["X-Content-Type-Options"] = "nosniff";
            }

            // Enable XSS protection (legacy browsers)
            if (!headers.ContainsKey("X-XSS-Protection"))
            {
                headers["X-XSS-Protection"] = "1; mode=block";
            }

            // Prevent information disclosure
            if (!headers.ContainsKey("X-Powered-By"))
            {
                headers["X-Powered-By"] = "Zeus.Academia";
            }

            // Referrer policy - limit information leakage
            if (!headers.ContainsKey("Referrer-Policy"))
            {
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            }

            // Permissions policy - control browser features
            if (!headers.ContainsKey("Permissions-Policy"))
            {
                headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=(), usb=()";
            }

            // Content Security Policy for API (restrictive since it's an API)
            if (!headers.ContainsKey("Content-Security-Policy"))
            {
                headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'; base-uri 'none'";
            }

            // Strict Transport Security for HTTPS
            if (context.Request.IsHttps && !headers.ContainsKey("Strict-Transport-Security"))
            {
                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
            }

            // Cache control for sensitive endpoints
            if (IsSensitiveEndpoint(context.Request.Path))
            {
                headers["Cache-Control"] = "no-store, no-cache, must-revalidate, private";
                headers["Pragma"] = "no-cache";
                headers["Expires"] = "0";
            }

            _logger.LogDebug("Security headers added to response for {Path}", context.Request.Path);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add some security headers for {Path}", context.Request.Path);
        }
    }

    private static bool IsSensitiveEndpoint(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant();
        return pathValue != null && (
            pathValue.Contains("/auth/") ||
            pathValue.Contains("/user/") ||
            pathValue.Contains("/admin/") ||
            pathValue.Contains("/token/") ||
            pathValue.Contains("/password/")
        );
    }
}

/// <summary>
/// Extension methods for registering SecurityHeadersMiddleware.
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    /// <summary>
    /// Adds the SecurityHeadersMiddleware to the application pipeline.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}