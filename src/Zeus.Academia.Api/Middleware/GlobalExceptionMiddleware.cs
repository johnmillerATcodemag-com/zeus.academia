using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using Zeus.Academia.Api.Configuration;
using Zeus.Academia.Api.Exceptions;
using Zeus.Academia.Api.Models.Responses;

namespace Zeus.Academia.Api.Middleware;

/// <summary>
/// Middleware for handling global exceptions and providing consistent error responses
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly ApiConfiguration _apiConfig;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Correlation ID header name
    /// </summary>
    public const string CorrelationIdHeader = "X-Correlation-ID";

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment,
        IOptions<ApiConfiguration> apiConfig)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
        _apiConfig = apiConfig.Value;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Ensure correlation ID is available
            EnsureCorrelationId(context);

            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private void EnsureCorrelationId(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers[CorrelationIdHeader] = correlationId;
        }

        // Add to response headers for client tracking
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        // Add to logging scope for all subsequent log entries
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestPath"] = context.Request.Path,
            ["RequestMethod"] = context.Request.Method
        });
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault() ?? Guid.NewGuid().ToString();
        var path = context.Request.Path.Value ?? string.Empty;

        // Log the exception with correlation ID
        LogException(exception, correlationId, context);

        // Create appropriate error response based on exception type
        var errorResponse = CreateErrorResponse(exception, correlationId, path);

        // Set response properties
        context.Response.StatusCode = errorResponse.Status;
        context.Response.ContentType = "application/json";

        // Ensure correlation ID is in response headers
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        // Serialize and write response
        var jsonResponse = JsonSerializer.Serialize(errorResponse, _jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }

    private void LogException(Exception exception, string correlationId, HttpContext context)
    {
        var logLevel = GetLogLevel(exception);
        var userId = context.User?.Identity?.Name ?? "Anonymous";
        var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["UserId"] = userId,
            ["UserAgent"] = userAgent,
            ["IpAddress"] = ipAddress,
            ["RequestPath"] = context.Request.Path,
            ["RequestMethod"] = context.Request.Method,
            ["ExceptionType"] = exception.GetType().Name
        });

        switch (logLevel)
        {
            case LogLevel.Error:
                _logger.LogError(exception,
                    "Unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}, Method: {Method}",
                    correlationId, context.Request.Path, context.Request.Method);
                break;

            case LogLevel.Warning:
                _logger.LogWarning(exception,
                    "Expected exception occurred. CorrelationId: {CorrelationId}, Path: {Path}, Method: {Method}",
                    correlationId, context.Request.Path, context.Request.Method);
                break;

            case LogLevel.Information:
                _logger.LogInformation(exception,
                    "Business exception occurred. CorrelationId: {CorrelationId}, Path: {Path}, Method: {Method}",
                    correlationId, context.Request.Path, context.Request.Method);
                break;
        }
    }

    private LogLevel GetLogLevel(Exception exception)
    {
        return exception switch
        {
            ValidationException => LogLevel.Information,
            BusinessLogicException => LogLevel.Information,
            NotAuthorizedException => LogLevel.Warning,
            UnauthorizedAccessException => LogLevel.Warning,
            ArgumentException => LogLevel.Information,
            _ => LogLevel.Error
        };
    }

    private ErrorResponse CreateErrorResponse(Exception exception, string correlationId, string path)
    {
        var includeDetails = ShouldIncludeDetails();

        return exception switch
        {
            ValidationException validationEx => ErrorResponseFactory.CreateValidationError(
                validationEx.Message,
                validationEx.ErrorCode,
                correlationId,
                path,
                validationEx.ValidationErrors.Any() ? validationEx.ValidationErrors : null,
                validationEx.Context.Any() ? validationEx.Context : null),

            BusinessLogicException businessEx => ErrorResponseFactory.CreateBusinessLogicError(
                businessEx.Message,
                businessEx.ErrorCode,
                correlationId,
                path,
                businessEx.Context.Any() ? businessEx.Context : null),

            NotAuthorizedException authEx => ErrorResponseFactory.CreateAuthorizationError(
                authEx.Message,
                authEx.ErrorCode,
                correlationId,
                path,
                authEx.Context.Any() ? authEx.Context : null),

            UnauthorizedAccessException => ErrorResponseFactory.CreateAuthorizationError(
                "Access denied. You do not have permission to perform this action.",
                "ACCESS_DENIED",
                correlationId,
                path),

            ArgumentException argEx => ErrorResponseFactory.CreateValidationError(
                argEx.Message,
                "INVALID_ARGUMENT",
                correlationId,
                path,
                context: new Dictionary<string, object> { ["parameter"] = argEx.ParamName ?? "unknown" }),

            TimeoutException => new ErrorResponse
            {
                Type = ErrorResponseFactory.ErrorTypes.TimeoutError,
                Code = "REQUEST_TIMEOUT",
                Message = "The request timed out. Please try again.",
                CorrelationId = correlationId,
                Path = path,
                Status = 408
            },

            TaskCanceledException => new ErrorResponse
            {
                Type = ErrorResponseFactory.ErrorTypes.TimeoutError,
                Code = "REQUEST_CANCELLED",
                Message = "The request was cancelled or timed out.",
                CorrelationId = correlationId,
                Path = path,
                Status = 408
            },

            _ => ErrorResponseFactory.CreateInternalServerError(
                correlationId,
                path,
                includeDetails,
                exception)
        };
    }

    private bool ShouldIncludeDetails()
    {
        // Include details in development or when explicitly configured
        return _environment.IsDevelopment() || _apiConfig.ShowDetailedErrors;
    }
}

/// <summary>
/// Extension methods for adding the GlobalExceptionMiddleware to the pipeline
/// </summary>
public static class GlobalExceptionMiddlewareExtensions
{
    /// <summary>
    /// Adds the global exception handling middleware to the pipeline
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}