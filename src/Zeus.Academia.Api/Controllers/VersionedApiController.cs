using Microsoft.AspNetCore.Mvc;
using Zeus.Academia.Api.Versioning;

namespace Zeus.Academia.Api.Controllers;

/// <summary>
/// Base controller for versioned API endpoints
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class VersionedApiController : ControllerBase
{
    /// <summary>
    /// Gets the current API version from the request context
    /// </summary>
    protected string CurrentApiVersion => HttpContext?.Items["ApiVersion"]?.ToString() ?? "1.0";

    /// <summary>
    /// Gets version information for API responses
    /// </summary>
    protected object GetVersionInfo() => new
    {
        ApiVersion = CurrentApiVersion,
        Timestamp = DateTime.UtcNow,
        RequestId = HttpContext?.TraceIdentifier
    };

    /// <summary>
    /// Creates a versioned response with metadata
    /// </summary>
    /// <typeparam name="T">The response data type</typeparam>
    /// <param name="data">The response data</param>
    /// <param name="message">Optional message</param>
    /// <returns>Versioned response object</returns>
    protected IActionResult VersionedResponse<T>(T data, string? message = null)
    {
        var response = new
        {
            Data = data,
            Message = message,
            Success = true,
            Version = GetVersionInfo()
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a versioned error response
    /// </summary>
    /// <param name="error">The error message</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <returns>Versioned error response</returns>
    protected IActionResult VersionedErrorResponse(string error, int statusCode = 400)
    {
        var response = new
        {
            Error = error,
            Success = false,
            Version = GetVersionInfo()
        };

        return StatusCode(statusCode, response);
    }
}