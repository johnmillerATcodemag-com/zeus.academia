using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net;

namespace Zeus.Academia.Api.Middleware;

/// <summary>
/// Configuration options for rate limiting.
/// </summary>
public class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    /// <summary>
    /// Maximum number of requests per time window for authentication endpoints.
    /// </summary>
    public int AuthenticationRequests { get; set; } = 5;

    /// <summary>
    /// Maximum number of requests per time window for general endpoints.
    /// </summary>
    public int GeneralRequests { get; set; } = 100;

    /// <summary>
    /// Time window in minutes for rate limiting.
    /// </summary>
    public int WindowMinutes { get; set; } = 15;

    /// <summary>
    /// Time to block IP address after exceeding limits (in minutes).
    /// </summary>
    public int BlockDurationMinutes { get; set; } = 60;
}

/// <summary>
/// Rate limiting data for tracking requests.
/// </summary>
internal class RateLimitData
{
    public int RequestCount { get; set; }
    public DateTime WindowStart { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime? BlockedUntil { get; set; }
}

/// <summary>
/// Middleware for implementing rate limiting to prevent brute force attacks.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;
    private readonly ConcurrentDictionary<string, RateLimitData> _rateLimitStore;
    private readonly Timer _cleanupTimer;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        IOptions<RateLimitOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
        _rateLimitStore = new ConcurrentDictionary<string, RateLimitData>();

        // Cleanup timer to remove expired entries every 5 minutes
        _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var isAuthEndpoint = IsAuthenticationEndpoint(context.Request.Path);

        // Check if client is currently blocked
        if (IsClientBlocked(clientId))
        {
            await HandleBlockedClient(context, clientId);
            return;
        }

        // Check rate limits
        if (IsRateLimitExceeded(clientId, isAuthEndpoint))
        {
            await HandleRateLimitExceeded(context, clientId, isAuthEndpoint);
            return;
        }

        // Update request count
        UpdateRequestCount(clientId, isAuthEndpoint);

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Prefer user ID if authenticated, otherwise use IP address
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user:{userId}";
        }

        // Get client IP address
        var clientIp = GetClientIpAddress(context);
        return $"ip:{clientIp}";
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Check X-Forwarded-For header first (for load balancers/proxies)
        var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            return xForwardedFor.Split(',').FirstOrDefault()?.Trim() ?? "unknown";
        }

        // Check X-Real-IP header
        var xRealIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp))
        {
            return xRealIp;
        }

        // Fall back to connection remote IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static bool IsAuthenticationEndpoint(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant();
        return pathValue != null && (
            pathValue.Contains("/auth/") ||
            pathValue.Contains("/login") ||
            pathValue.Contains("/token") ||
            pathValue.Contains("/password/reset") ||
            pathValue.Contains("/password/forgot")
        );
    }

    private bool IsClientBlocked(string clientId)
    {
        if (!_rateLimitStore.TryGetValue(clientId, out var data))
            return false;

        if (data.IsBlocked && data.BlockedUntil.HasValue)
        {
            if (DateTime.UtcNow < data.BlockedUntil.Value)
            {
                return true;
            }

            // Unblock client if block period has expired
            data.IsBlocked = false;
            data.BlockedUntil = null;
            data.RequestCount = 0;
            data.WindowStart = DateTime.UtcNow;
        }

        return false;
    }

    private bool IsRateLimitExceeded(string clientId, bool isAuthEndpoint)
    {
        var now = DateTime.UtcNow;
        var data = _rateLimitStore.GetOrAdd(clientId, _ => new RateLimitData { WindowStart = now });

        // Check if we need to reset the window
        if (now - data.WindowStart > TimeSpan.FromMinutes(_options.WindowMinutes))
        {
            data.RequestCount = 0;
            data.WindowStart = now;
        }

        var limit = isAuthEndpoint ? _options.AuthenticationRequests : _options.GeneralRequests;
        return data.RequestCount >= limit;
    }

    private void UpdateRequestCount(string clientId, bool isAuthEndpoint)
    {
        var now = DateTime.UtcNow;
        var data = _rateLimitStore.GetOrAdd(clientId, _ => new RateLimitData { WindowStart = now });

        // Reset window if needed
        if (now - data.WindowStart > TimeSpan.FromMinutes(_options.WindowMinutes))
        {
            data.RequestCount = 0;
            data.WindowStart = now;
        }

        data.RequestCount++;
    }

    private async Task HandleBlockedClient(HttpContext context, string clientId)
    {
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.Response.Headers["Retry-After"] = (_options.BlockDurationMinutes * 60).ToString();

        var response = new
        {
            error = "Too many requests",
            message = $"Client is temporarily blocked. Try again after {_options.BlockDurationMinutes} minutes.",
            retryAfter = _options.BlockDurationMinutes * 60
        };

        _logger.LogWarning("Blocked client attempted request: {ClientId} from {Path}",
            clientId, context.Request.Path);

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }

    private async Task HandleRateLimitExceeded(HttpContext context, string clientId, bool isAuthEndpoint)
    {
        // Block client if this is an authentication endpoint
        if (isAuthEndpoint)
        {
            var data = _rateLimitStore.GetOrAdd(clientId, _ => new RateLimitData());
            data.IsBlocked = true;
            data.BlockedUntil = DateTime.UtcNow.AddMinutes(_options.BlockDurationMinutes);

            _logger.LogWarning("Client blocked due to excessive authentication attempts: {ClientId} from {Path}",
                clientId, context.Request.Path);
        }

        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.Response.Headers["Retry-After"] = "900"; // 15 minutes default

        var limit = isAuthEndpoint ? _options.AuthenticationRequests : _options.GeneralRequests;
        var response = new
        {
            error = "Rate limit exceeded",
            message = $"Too many requests. Limit: {limit} per {_options.WindowMinutes} minutes.",
            retryAfter = _options.WindowMinutes * 60
        };

        _logger.LogWarning("Rate limit exceeded: {ClientId} from {Path}, IsAuth: {IsAuth}",
            clientId, context.Request.Path, isAuthEndpoint);

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }

    private void CleanupExpiredEntries(object? state)
    {
        try
        {
            var now = DateTime.UtcNow;
            var expiredKeys = new List<string>();

            foreach (var kvp in _rateLimitStore)
            {
                var data = kvp.Value;

                // Remove entries that are expired and not blocked
                if (!data.IsBlocked &&
                    now - data.WindowStart > TimeSpan.FromMinutes(_options.WindowMinutes * 2))
                {
                    expiredKeys.Add(kvp.Key);
                }
                // Remove blocked entries that have expired
                else if (data.IsBlocked &&
                         data.BlockedUntil.HasValue &&
                         now > data.BlockedUntil.Value.AddMinutes(30))
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                _rateLimitStore.TryRemove(key, out _);
            }

            if (expiredKeys.Count > 0)
            {
                _logger.LogDebug("Cleaned up {Count} expired rate limit entries", expiredKeys.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during rate limit cleanup");
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}

/// <summary>
/// Extension methods for registering RateLimitingMiddleware.
/// </summary>
public static class RateLimitingMiddlewareExtensions
{
    /// <summary>
    /// Adds the RateLimitingMiddleware to the application pipeline.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}