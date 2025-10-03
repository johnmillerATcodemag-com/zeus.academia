using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zeus.Academia.Api.Models.Common;
using Zeus.Academia.Api.Models.Responses;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Extensions;

namespace Zeus.Academia.Api.Controllers;

/// <summary>
/// Base or Zeus Academia API with common functionality and authentication support
/// </summary>
/// <remarks>
/// This controller provides common functionality for all API controllers including:
/// - User authentication and claims access
/// - Standard response formatting
/// - Error handling helpers
/// - Security utilities
/// </remarks>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")] // Also support header-based versioning
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
        HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

    /// <summary>
    /// Gets the API version for the current request
    /// </summary>
    protected string? GetApiVersion()
    {
        // Try to get version from various sources
        if (HttpContext?.Items?.ContainsKey("ApiVersion") == true)
        {
            return HttpContext.Items["ApiVersion"]?.ToString();
        }

        if (HttpContext?.Request?.Headers?.ContainsKey("X-Api-Version") == true)
        {
            return HttpContext.Request.Headers["X-Api-Version"].FirstOrDefault();
        }

        if (HttpContext?.Request?.Query?.ContainsKey("version") == true)
        {
            return HttpContext.Request.Query["version"].FirstOrDefault();
        }

        // Default to v1.0 if no version specified
        return "1.0";
    }

    /// <summary>
    /// Creates a success response with data
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="data">Response data</param>
    /// <param name="message">Optional success message</param>
    /// <returns>OK result with formatted response</returns>
    protected IActionResult Success<T>(T data, string? message = null)
    {
        var response = ApiResponse<T>.CreateSuccess(
            data,
            message ?? "Request successful",
            CorrelationId,
            GetApiVersion()
        );

        return Ok(response);
    }

    /// <summary>
    /// Creates a success response without data
    /// </summary>
    /// <param name="message">Success message</param>
    /// <returns>OK result with formatted response</returns>
    protected IActionResult Success(string message = "Operation completed successfully")
    {
        var response = ApiResponse.CreateSuccess(
            message,
            CorrelationId,
            GetApiVersion()
        );

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
        var response = ApiResponse.CreateError(
            message,
            errors as Dictionary<string, string[]>,
            CorrelationId,
            GetApiVersion()
        );

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

    /// <summary>
    /// Creates a successful paged response
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    /// <param name="data">Page data</param>
    /// <param name="currentPage">Current page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="totalItems">Total number of items</param>
    /// <param name="message">Optional success message</param>
    /// <returns>OK result with paged response</returns>
    protected IActionResult PagedSuccess<T>(
        IEnumerable<T> data,
        int currentPage,
        int pageSize,
        int totalItems,
        string? message = null)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var response = PagedApiResponse<T>.CreateSuccess(
            data,
            currentPage,
            pageSize,
            totalItems,
            message,
            CorrelationId,
            GetApiVersion());
#pragma warning restore CS0618 // Type or member is obsolete

        return Ok(response);
    }

    /// <summary>
    /// Creates a paged success response using PagedResult
    /// </summary>
    /// <typeparam name="T">Type of data items</typeparam>
    /// <param name="pagedResult">Paged result containing data and pagination metadata</param>
    /// <param name="message">Optional success message</param>
    /// <returns>OK result with paged response</returns>
    protected IActionResult PagedSuccess<T>(PagedResult<T> pagedResult, string? message = null)
    {
        if (pagedResult?.Pagination == null)
        {
            return Error("Invalid paged result", 500);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        var response = PagedApiResponse<T>.CreateSuccess(
            pagedResult.Items,
            pagedResult.Pagination,
            message,
            CorrelationId,
            GetApiVersion());
#pragma warning restore CS0618 // Type or member is obsolete

        return Ok(response);
    }

    /// <summary>
    /// Creates a success response with data
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <param name="data">Response data</param>
    /// <param name="message">Optional success message</param>
    /// <returns>Formatted success response</returns>
    protected IActionResult CreateSuccessResponse<T>(T data, string? message = null)
    {
        return Success(data, message);
    }

    /// <summary>
    /// Creates a success response without data
    /// </summary>
    /// <param name="message">Success message</param>
    /// <returns>Formatted success response</returns>
    protected IActionResult CreateSuccessResponse(string message = "Operation completed successfully")
    {
        return Success(message);
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="errors">Detailed error information</param>
    /// <returns>Formatted error response</returns>
    protected IActionResult CreateErrorResponse(string message, int statusCode = 400, object? errors = null)
    {
        return Error(message, statusCode, errors);
    }

    /// <summary>
    /// Creates a validation error response
    /// </summary>
    /// <param name="errors">Validation errors</param>
    /// <param name="message">Optional custom message</param>
    /// <returns>Formatted validation error response</returns>
    protected IActionResult CreateValidationErrorResponse(object errors, string? message = null)
    {
        return Error(message ?? "Validation failed", 400, errors);
    }

}