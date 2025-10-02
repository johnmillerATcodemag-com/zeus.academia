using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Zeus.Academia.Api.Services;

namespace Zeus.Academia.Api.Middleware;

/// <summary>
/// Middleware for comprehensive HTTP request/response logging and performance monitoring.
/// Captures request details, response details, execution time, and includes correlation ID tracking.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly ICorrelationIdService _correlationIdService;
    private readonly RequestLoggingOptions _options;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger,
        ICorrelationIdService correlationIdService,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _correlationIdService = correlationIdService;
        _options = configuration.GetSection(RequestLoggingOptions.SectionName)
            .Get<RequestLoggingOptions>() ?? new RequestLoggingOptions();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip logging for excluded paths
        if (ShouldSkipLogging(context))
        {
            await _next(context);
            return;
        }

        // Generate or extract correlation ID
        var correlationId = GetOrGenerateCorrelationId(context);
        _correlationIdService.SetCorrelationId(correlationId);

        // Add correlation ID to response headers
        context.Response.Headers.TryAdd("X-Correlation-ID", correlationId);

        // Start performance monitoring
        var stopwatch = Stopwatch.StartNew();
        var requestBody = string.Empty;
        var responseBody = string.Empty;

        try
        {
            // Capture request details
            if (_options.LogRequestBody && HasBody(context.Request))
            {
                requestBody = await CaptureRequestBodyAsync(context);
            }

            // Log incoming request
            LogRequest(context, requestBody, correlationId);

            // Capture response body if configured
            if (_options.LogResponseBody)
            {
                var originalBodyStream = context.Response.Body;
                using var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                // Execute the request
                await _next(context);

                // Capture response body
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
                responseBodyStream.Seek(0, SeekOrigin.Begin);

                // Copy response back to original stream
                await responseBodyStream.CopyToAsync(originalBodyStream);
            }
            else
            {
                // Execute the request without capturing response body
                await _next(context);
            }

            stopwatch.Stop();

            // Log successful response
            LogResponse(context, responseBody, correlationId, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log error response
            LogErrorResponse(context, ex, correlationId, stopwatch.ElapsedMilliseconds);

            throw; // Re-throw to let the exception middleware handle it
        }
    }

    private string GetOrGenerateCorrelationId(HttpContext context)
    {
        // Check if correlation ID is provided in headers
        if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var headerValue) &&
            !string.IsNullOrEmpty(headerValue.FirstOrDefault()))
        {
            return headerValue.FirstOrDefault()!;
        }

        // Check if correlation ID is provided in query parameters (for debugging)
        if (context.Request.Query.TryGetValue("correlationId", out var queryValue) &&
            !string.IsNullOrEmpty(queryValue.FirstOrDefault()))
        {
            return queryValue.FirstOrDefault()!;
        }

        // Generate new correlation ID
        return _correlationIdService.GenerateCorrelationId();
    }

    private bool ShouldSkipLogging(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

        return _options.ExcludedPaths.Any(excludedPath =>
            path.StartsWith(excludedPath.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase));
    }

    private static bool HasBody(HttpRequest request)
    {
        return request.ContentLength > 0 &&
               !string.IsNullOrEmpty(request.ContentType) &&
               (request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
                request.ContentType.Contains("application/xml", StringComparison.OrdinalIgnoreCase) ||
                request.ContentType.Contains("text/", StringComparison.OrdinalIgnoreCase));
    }

    private async Task<string> CaptureRequestBodyAsync(HttpContext context)
    {
        try
        {
            context.Request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength ?? 0)];
            await context.Request.Body.ReadExactlyAsync(buffer, 0, buffer.Length);

            var requestBody = Encoding.UTF8.GetString(buffer);
            context.Request.Body.Position = 0; // Reset for next middleware

            return TruncateIfNeeded(requestBody, _options.MaxBodyLength);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to capture request body for correlation ID {CorrelationId}",
                _correlationIdService.CorrelationId);
            return "[Failed to capture request body]";
        }
    }

    private void LogRequest(HttpContext context, string requestBody, string correlationId)
    {
        var request = context.Request;

        _logger.LogInformation(
            "HTTP Request Started - {Method} {Path} | Correlation ID: {CorrelationId} | IP: {ClientIP} | User Agent: {UserAgent} | Content Type: {ContentType} | Content Length: {ContentLength}",
            request.Method,
            $"{request.Path}{request.QueryString}",
            correlationId,
            GetClientIpAddress(context),
            request.Headers.UserAgent.FirstOrDefault() ?? "Unknown",
            request.ContentType ?? "None",
            request.ContentLength ?? 0);

        // Log request body if available and not sensitive
        if (!string.IsNullOrEmpty(requestBody) && !ContainsSensitiveData(requestBody))
        {
            _logger.LogDebug(
                "HTTP Request Body - Correlation ID: {CorrelationId} | Body: {RequestBody}",
                correlationId,
                requestBody);
        }

        // Log request headers if in debug mode
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var headers = request.Headers
                .Where(h => !_options.SensitiveHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => string.Join(", ", h.Value.ToArray()));

            _logger.LogDebug(
                "HTTP Request Headers - Correlation ID: {CorrelationId} | Headers: {Headers}",
                correlationId,
                JsonSerializer.Serialize(headers));
        }
    }

    private void LogResponse(HttpContext context, string responseBody, string correlationId, long elapsedMs)
    {
        var response = context.Response;
        var logLevel = GetLogLevelForStatusCode(response.StatusCode);

        _logger.Log(logLevel,
            "HTTP Request Completed - {Method} {Path} | Status: {StatusCode} | Correlation ID: {CorrelationId} | Duration: {ElapsedMs}ms | Content Type: {ContentType} | Content Length: {ContentLength}",
            context.Request.Method,
            $"{context.Request.Path}{context.Request.QueryString}",
            response.StatusCode,
            correlationId,
            elapsedMs,
            response.ContentType ?? "None",
            response.ContentLength ?? 0);

        // Log response body if available, successful, and not sensitive
        if (!string.IsNullOrEmpty(responseBody) &&
            response.StatusCode < 400 &&
            !ContainsSensitiveData(responseBody))
        {
            _logger.LogDebug(
                "HTTP Response Body - Correlation ID: {CorrelationId} | Status: {StatusCode} | Body: {ResponseBody}",
                correlationId,
                response.StatusCode,
                responseBody);
        }

        // Log performance metrics
        if (elapsedMs > _options.SlowRequestThresholdMs)
        {
            _logger.LogWarning(
                "Slow HTTP Request Detected - {Method} {Path} | Duration: {ElapsedMs}ms | Correlation ID: {CorrelationId}",
                context.Request.Method,
                $"{context.Request.Path}{context.Request.QueryString}",
                elapsedMs,
                correlationId);
        }
    }

    private void LogErrorResponse(HttpContext context, Exception exception, string correlationId, long elapsedMs)
    {
        _logger.LogError(exception,
            "HTTP Request Failed - {Method} {Path} | Correlation ID: {CorrelationId} | Duration: {ElapsedMs}ms | Error: {ErrorMessage}",
            context.Request.Method,
            $"{context.Request.Path}{context.Request.QueryString}",
            correlationId,
            elapsedMs,
            exception.Message);
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP (reverse proxy scenarios)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private static LogLevel GetLogLevelForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warning,
            >= 300 => LogLevel.Information,
            _ => LogLevel.Information
        };
    }

    private bool ContainsSensitiveData(string content)
    {
        if (string.IsNullOrEmpty(content))
            return false;

        var lowerContent = content.ToLowerInvariant();
        return _options.SensitiveDataPatterns.Any(pattern =>
            lowerContent.Contains(pattern.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase));
    }

    private string TruncateIfNeeded(string content, int maxLength)
    {
        if (string.IsNullOrEmpty(content) || content.Length <= maxLength)
            return content;

        return content[..maxLength] + "... [truncated]";
    }
}

/// <summary>
/// Configuration options for request logging middleware.
/// </summary>
public class RequestLoggingOptions
{
    public const string SectionName = "RequestLogging";

    /// <summary>
    /// Whether to log request bodies. Default: false (for performance and security).
    /// </summary>
    public bool LogRequestBody { get; set; } = false;

    /// <summary>
    /// Whether to log response bodies. Default: false (for performance and security).
    /// </summary>
    public bool LogResponseBody { get; set; } = false;

    /// <summary>
    /// Maximum length of request/response body to log. Default: 1024 characters.
    /// </summary>
    public int MaxBodyLength { get; set; } = 1024;

    /// <summary>
    /// Threshold in milliseconds to consider a request as slow. Default: 1000ms.
    /// </summary>
    public int SlowRequestThresholdMs { get; set; } = 1000;

    /// <summary>
    /// Paths to exclude from logging (e.g., health checks, metrics).
    /// </summary>
    public List<string> ExcludedPaths { get; set; } = new()
    {
        "/health",
        "/metrics",
        "/favicon.ico",
        "/swagger",
        "/_vs/"
    };

    /// <summary>
    /// Headers considered sensitive and should not be logged.
    /// </summary>
    public List<string> SensitiveHeaders { get; set; } = new()
    {
        "Authorization",
        "Cookie",
        "X-Api-Key",
        "X-Auth-Token"
    };

    /// <summary>
    /// Patterns to identify sensitive data in request/response bodies.
    /// </summary>
    public List<string> SensitiveDataPatterns { get; set; } = new()
    {
        "password",
        "token",
        "secret",
        "key",
        "credential",
        "ssn",
        "social security"
    };
}