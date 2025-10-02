using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Zeus.Academia.Api.Controllers;

/// <summary>
/// Base controller for Zeus Academia API with common functionality
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Gets the current user's ID from the JWT token
    /// </summary>
    protected int? CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// Gets the current user's username from the JWT token
    /// </summary>
    protected string? CurrentUserName => User.FindFirst(ClaimTypes.Name)?.Value;

    /// <summary>
    /// Gets the current user's email from the JWT token
    /// </summary>
    protected string? CurrentUserEmail => User.FindFirst(ClaimTypes.Email)?.Value;

    /// <summary>
    /// Gets the current user's roles from the JWT token
    /// </summary>
    protected IEnumerable<string> CurrentUserRoles =>
        User.FindAll(ClaimTypes.Role).Select(c => c.Value);

    /// <summary>
    /// Gets the current user's permissions from the JWT token
    /// </summary>
    protected IEnumerable<string> CurrentUserPermissions =>
        User.FindAll("permission").Select(c => c.Value);

    /// <summary>
    /// Gets the client IP address from the request
    /// </summary>
    protected string? ClientIpAddress
    {
        get
        {
            // Check for forwarded IP first (reverse proxy scenarios)
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }

    /// <summary>
    /// Gets the user agent from the request
    /// </summary>
    protected string? UserAgent => Request.Headers["User-Agent"].FirstOrDefault();

    /// <summary>
    /// Gets the correlation ID for the current request
    /// </summary>
    protected string CorrelationId =>
        HttpContext.TraceIdentifier ?? Guid.NewGuid().ToString();

    /// <summary>
    /// Creates a success response with data
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="data">Response data</param>
    /// <param name="message">Optional success message</param>
    /// <returns>OK result with formatted response</returns>
    protected IActionResult Success<T>(T data, string? message = null)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Timestamp = DateTime.UtcNow,
            CorrelationId = CorrelationId
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a success response without data
    /// </summary>
    /// <param name="message">Success message</param>
    /// <returns>OK result with formatted response</returns>
    protected IActionResult Success(string message = "Operation completed successfully")
    {
        var response = new ApiResponse
        {
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow,
            CorrelationId = CorrelationId
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="errors">Detailed error information</param>
    /// <returns>Error result with formatted response</returns>
    protected IActionResult Error(string message, int statusCode = 400, object? errors = null)
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow,
            CorrelationId = CorrelationId
        };

        return StatusCode(statusCode, response);
    }

    /// <summary>
    /// Creates a validation error response
    /// </summary>
    /// <param name="errors">Validation errors</param>
    /// <returns>Bad request result with validation errors</returns>
    protected IActionResult ValidationError(object errors)
    {
        return Error("Validation failed", 400, errors);
    }

    /// <summary>
    /// Creates a not found response
    /// </summary>
    /// <param name="message">Not found message</param>
    /// <returns>Not found result</returns>
    protected IActionResult NotFound(string message = "Resource not found")
    {
        return Error(message, 404);
    }

    /// <summary>
    /// Creates an unauthorized response
    /// </summary>
    /// <param name="message">Unauthorized message</param>
    /// <returns>Unauthorized result</returns>
    protected IActionResult Unauthorized(string message = "Access denied")
    {
        return Error(message, 401);
    }

    /// <summary>
    /// Creates a forbidden response
    /// </summary>
    /// <param name="message">Forbidden message</param>
    /// <returns>Forbidden result</returns>
    protected IActionResult Forbidden(string message = "Insufficient permissions")
    {
        return Error(message, 403);
    }
}

/// <summary>
/// Generic API response wrapper
/// </summary>
/// <typeparam name="T">Type of response data</typeparam>
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Response data
    /// </summary>
    public T? Data { get; set; }
}

/// <summary>
/// Base API response wrapper
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Error details (only populated on failure)
    /// </summary>
    public object? Errors { get; set; }

    /// <summary>
    /// Response timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Correlation ID for request tracking
    /// </summary>
    public string? CorrelationId { get; set; }
}